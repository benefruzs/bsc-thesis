using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using DocEditor.Model;
using DocEditor.ViewModel;
using DocEditor.Parser;
using Microsoft.Win32;

namespace DocEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Private fields
        struct SelectionOffsets { internal int Start; internal int End; }
        private DocEditorModel _model;
        private ParserMain _parser;
        private MainViewModel _viewModel;
        private MainWindow _view;
        private HomeWindow _home;
        private PdfViewer _dictHelp;

        private FormatTextViewModel _ftViewModel;
        private PageSettingsViewModel _pageViewModel;
        private DictionaryViewModel _dictViewModel;
        private InsertViewModel _insViewModel;

        private List<FlowDocument> DocPapers; //the current richtextbox

        private SelectionOffsets selectionOffsets;

        private CreateFileDialog _createJsonDialog;
        private ErrorCorrectViewModel _ecViewModel;
        private ErrorCorrectDialogWindow _ecDialog;
        private PageSettingsWindow _psWindow;

        #endregion

        #region Application startup
        /// <summary>
        /// Instantiate the application
        /// </summary>
        public App()
        {
            Startup += App_Startup;
        }

        /// <summary>
        /// Event handler for starting the application
        /// </summary>
        private void App_Startup(object sender, StartupEventArgs e)
        {
            _model = new DocEditorModel();

            _parser = new ParserMain();

            _viewModel = new MainViewModel(_model);
            _viewModel.ExitApp += new EventHandler(ViewModel_Exit);
            _viewModel.MinApp += new EventHandler(ViewModel_Min);
            _viewModel.MaxApp += new EventHandler(ViewModel_Max);

            _viewModel.OpenHome += new EventHandler(ViewModel_OpenHomeWindow);
            _viewModel.CloseHome += new EventHandler(Home_Close);
            _viewModel.MinHome += new EventHandler(Home_Min);
            _viewModel.MaxHome += new EventHandler(Home_Max);
            _viewModel.NewEmptyFile += new EventHandler(Home_NewEmptyFile);

            _viewModel.OpenFormatText += new EventHandler(ViewModel_FormatText);
            _viewModel.OpenEditPage += new EventHandler(ViewModel_EditPage);
            _viewModel.OpenInsert += new EventHandler(ViewModel_Insert);
            _viewModel.OpenDict += new EventHandler(ViewModel_Dict);


            _viewModel.OpenDictHelp += new EventHandler(ViewModel_OpenDictHelp);
            _viewModel.DeleteSelection += new EventHandler(ViewModel_DeleteSelection);
            _viewModel.SelectAllText += new EventHandler(ViewModel_SelectAllText);
            _viewModel.NewPlainDocument += new EventHandler(ViewModel_NewPlainDocument);
            _viewModel.OpenDictionaryFile += new EventHandler(OpenNewDictFile);

            _viewModel.SelectChanged += new EventHandler(setSelection);
            _viewModel.AutoFormat += new EventHandler(AutoFormatting_EventHandler);
            _viewModel.NewParagraph += new EventHandler(AddNewParagraph);

            _viewModel.UpdateRTB += new EventHandler(updateRTBList); //TODO
            _viewModel.GotoFirstPage += new EventHandler(Doc_GotoFirst);
            _viewModel.GotoLastPage += new EventHandler(Doc_GotoLast);
            _viewModel.GotoNextPage += new EventHandler(Doc_GotoNext);
            _viewModel.GotoPrevPage += new EventHandler(Doc_GotoPrev);

            _ftViewModel = new FormatTextViewModel(_model);
            _ftViewModel.OpenColorPicker += new EventHandler(openColorDialog);
            _ftViewModel.AddMoreFormatting += new EventHandler(openMoreFormatting);
            _ftViewModel.AddMoreSpacing += new EventHandler(openMoreSpacing);

            _ftViewModel.FontFamilyChanged += new EventHandler(Change_FontFamily);
            _ftViewModel.FontSizeChanged += new EventHandler(Change_FontSize);
            _ftViewModel.RTBFocus += new EventHandler(RTB_GetFocusBack);
            _ftViewModel.SetBold += new EventHandler(Selection_SetBold);
            _ftViewModel.SetItalic += new EventHandler(Selection_SetItalic);
            _ftViewModel.SetUnderLine += new EventHandler(Selection_SetUnderline);
            _ftViewModel.SetStrikeThrough += new EventHandler(Selection_SetStrikeThrough);
            _ftViewModel.SetSuperScript += new EventHandler(Selection_SetSuperScript);
            _ftViewModel.SetSubScript += new EventHandler(Selection_SetSubScript);
            _ftViewModel.StyleChanged += new EventHandler(Selection_AddFormatStyle);
            _ftViewModel.NewStyleAdded += new EventHandler(AddNewStyle);
            _ftViewModel.DeleteFormatting += new EventHandler(DeleteFormatting);
            _ftViewModel.AddLineHeight += new EventHandler(ViewModel_AddLineHeight);
            _ftViewModel.LessLineHeight += new EventHandler(ViewModel_LessLineHeight);

            _ftViewModel.CenterAlign += new EventHandler(SetCenterAlignment);
            _ftViewModel.LeftAlign += new EventHandler(SetLeftAlignment);
            _ftViewModel.RightAlign += new EventHandler(SetRightAlignment);
            _ftViewModel.JustifyAlign += new EventHandler(SetJustifyAlignment);

            _dictViewModel = new DictionaryViewModel(_model, _parser);
            _dictViewModel.AddToDictionary += new EventHandler(AddToDictionary_EventHandler);
            _dictViewModel.SetDictFileName += new EventHandler(SetJsonFileName);
            _dictViewModel.SelectedDictElementChanged += new EventHandler(DictPreview);
            _dictViewModel.RemoveEvent += new EventHandler(RemoveFromDictionary);

            _pageViewModel = new PageSettingsViewModel(_model);
            _pageViewModel.MorePageSettings += new EventHandler(openMorePageSettings);
            _pageViewModel.SetBottomMargin += new EventHandler(ChangeBottomMargin);
            _pageViewModel.SetTopMargin += new EventHandler(ChangeTopMargin);
            _pageViewModel.SetLeftMargin += new EventHandler(ChangeLeftMargin);
            _pageViewModel.SetRightMargin += new EventHandler(ChangeRightMargin);
            _pageViewModel.Set05Margin += new EventHandler(Set05MarginEventHandler);
            _pageViewModel.Set15Margin += new EventHandler(Set15MarginEventHandler);
            _pageViewModel.Set25Margin += new EventHandler(Set25MarginEventHandler);
            _pageViewModel.Set35Margin += new EventHandler(Set35MarginEventHandler);
            _pageViewModel.ClosePageSettings += new EventHandler(ClosePageSettingsEventHandler);
            _pageViewModel.OkPageSettings += new EventHandler(OkPageSettingsEventHandler);

            _ecViewModel = new ErrorCorrectViewModel(_model, _parser);
            _ecViewModel.NoThanksEvent += new EventHandler(EcViewModel_NoThanks);
            _ecViewModel.OkayEvent += new EventHandler(EcViewModel_Okay);
            _ecViewModel.SelectedPossibleDictElementChanged += new EventHandler(EcViewModel_SelectChanged);
            _ecDialog = new ErrorCorrectDialogWindow
            {
                DataContext = _ecViewModel
            };

            //EventBinding.Bind(_view.DocPaper, nameof(_view.DocPaper.SelectionChanged), setSelection);

            DocPapers = new List<FlowDocument>();

            _home = new HomeWindow
            {
                DataContext = _viewModel
            };
            _home.ShowDialog();
        }

        private void OkPageSettingsEventHandler(object sender, EventArgs e)
        {
            _model.SetBottomMargin(Double.Parse(_psWindow.bottom.Text));
            _model.SetTopMargin(Double.Parse(_psWindow.top.Text));
            _model.SetLeftMargin(Double.Parse(_psWindow.left.Text));
            _model.SetRightMargin(Double.Parse(_psWindow.right.Text));

            _viewModel.SetMarginsRTB();
            _pageViewModel.UpdateMargins();
            _psWindow.Close();
            SetFocus();
        }

        private void ClosePageSettingsEventHandler(object sender, EventArgs e)
        {
            _psWindow.Close();
            SetFocus();
        }

        private void Set35MarginEventHandler(object sender, EventArgs e)
        {
            _model.SetAllMargins(3.5);
            _viewModel.SetMarginsRTB();
            _pageViewModel.UpdateMargins();
            _psWindow.Close();
            SetFocus();

        }

        private void Set25MarginEventHandler(object sender, EventArgs e)
        {
            _model.SetAllMargins(2.5);
            _viewModel.SetMarginsRTB();
            _pageViewModel.UpdateMargins();
            _psWindow.Close();
            SetFocus();

        }

        private void Set15MarginEventHandler(object sender, EventArgs e)
        {
            _model.SetAllMargins(1.5);
            _viewModel.SetMarginsRTB();
            _pageViewModel.UpdateMargins();
            _psWindow.Close();
            SetFocus();
        }

        private void Set05MarginEventHandler(object sender, EventArgs e)
        {
            _model.SetAllMargins(0.5);
            _viewModel.SetMarginsRTB();
            _pageViewModel.UpdateMargins();
            _psWindow.Close();
            SetFocus();
        }

        private void ChangeRightMargin(object sender, EventArgs e)
        {
            SetFocus();
            _model.SetRightMargin(_pageViewModel.RightMargin);
            _viewModel.SetMarginsRTB();
            _pageViewModel.UpdateMargins();
        }

        private void ChangeLeftMargin(object sender, EventArgs e)
        {
            SetFocus();
            _model.SetLeftMargin(_pageViewModel.LeftMargin);
            _viewModel.SetMarginsRTB();
            _pageViewModel.UpdateMargins();
        }

        private void ChangeTopMargin(object sender, EventArgs e)
        {
            SetFocus();
            _model.SetTopMargin(_pageViewModel.TopMargin);
            _viewModel.SetMarginsRTB();
            _pageViewModel.UpdateMargins();
        }

        private void ChangeBottomMargin(object sender, EventArgs e)
        {
            SetFocus();
            _model.SetBottomMargin(_pageViewModel.BottomMargin);
            _viewModel.SetMarginsRTB();
            _pageViewModel.UpdateMargins();
        }

        private void ViewModel_LessLineHeight(object sender, EventArgs e)
        {
            _ftViewModel.DecreaseLineHeight();
            _view.DocPaper.Document.LineHeight = _ftViewModel.LineHeightProp;
            SetFocus();
        }

        private void ViewModel_AddLineHeight(object sender, EventArgs e)
        {
            _ftViewModel.IncreaseLineHeight();
            _view.DocPaper.Document.LineHeight = _ftViewModel.LineHeightProp;
            SetFocus();
        }

        #endregion

        #region Error correcting event handlers
        private void EcViewModel_SelectChanged(object sender, EventArgs e)
        {
            if (_ecViewModel.SelectedPossibleDictElement != null)
            {
                _ecViewModel.DictString = _ecViewModel.SelectedPossibleDictElement.Str;
                Selection sel = new Selection();
                Stwf st = new Stwf(sel, new FormatModel[_ecViewModel.DictString.Length]);

                FormatModel[] fm = _parser.GetFormatting(_ecViewModel.SelectedPossibleDictElement);
                _ecViewModel.DisplayText(fm);
            }
        }

        private void EcViewModel_Okay(object sender, EventArgs e)
        {
            TextRange tr = new TextRange(_view.DocPaper.Document.ContentStart.GetPositionAtOffset(_model.SelectForParser.SelectedText.StartPointer), _view.DocPaper.Document.ContentStart.GetPositionAtOffset(_model.SelectForParser.SelectedText.EndPointer));
            tr.Text = "";

            TextPointer tp = _view.DocPaper.Document.ContentStart.GetPositionAtOffset(_model.SelectForParser.SelectedText.StartPointer);
            _view.DocPaper.CaretPosition = tp;

            FormatModel[] fm = _parser.GetFormatting(_ecViewModel.SelectedPossibleDictElement);

            Selection newSel = _parser.GetPossibleFormatting(_ecViewModel.SelectedPossibleDictElement, _model.SelectForParser);
            _model.SelectForParser = new Stwf(newSel, fm);

            _view.DocPaper.CaretPosition.InsertTextInRun(_model.SelectForParser.SelectedText.SelectedString);

            tp = _view.DocPaper.Document.ContentStart.GetPositionAtOffset(_model.SelectForParser.SelectedText.EndPointer);
            _view.DocPaper.CaretPosition = tp.GetNextInsertionPosition(LogicalDirection.Forward);

            setFormatting(_model.SelectForParser);
            _ecDialog.Close();
        }

        private void EcViewModel_NoThanks(object sender, EventArgs e)
        {
            _ecDialog.Close();
        }
        #endregion

        #region ViewModel home window event handlers
        /// <summary>
        /// Home Window event handlers
        /// </summary>
        private void ViewModel_OpenHomeWindow(object sender, EventArgs e)
        {
            _home = new HomeWindow
            {
                DataContext = _viewModel
            };
            _home.ShowDialog();
        }

        private void Home_Close(object sender, EventArgs e)
        {
            if (_view == null)
            {
                Shutdown();
            }
            else
            {
                _home.Close();
            }
        }

        private void Home_Min(object sender, EventArgs e)
        {
            _home.WindowState = WindowState.Minimized;
        }

        private void Home_Max(object sender, EventArgs e)
        {
            if (_home.WindowState == WindowState.Normal)
            {
                _home.WindowState = WindowState.Normal;
            }
            else
            {
                _home.WindowState = WindowState.Normal;
            }
        }

        private void Home_NewEmptyFile(object sender, EventArgs e)
        {

            _view = new MainWindow
            {
                DataContext = _viewModel
            };
            _view.Show();
            _home.Close();
            //TODO
            //új üres file megnyitása itt a viewban
            NewEmpty();

        }

        #endregion

        #region ViewModel main window event handlers

        /// <summary>
        /// Main Window event handlers
        /// </summary>
        private void ViewModel_Exit(object sender, EventArgs e)
        {
            if (true) // a legutolsó mentés óta volt modify
            {
                MessageBoxResult result = MessageBox.Show("Kilép mentés nélkül? ", "DocEditor", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        Shutdown();
                        break;
                    case MessageBoxResult.No:
                        //save dialog megnyitása
                        break;
                    case MessageBoxResult.Cancel:
                        break;
                }
            }
            else
            {
                Shutdown();
            }
        }

        private void ViewModel_Min(object sender, EventArgs e)
        {
            _view.WindowState = WindowState.Minimized;
        }

        private void ViewModel_Max(object sender, EventArgs e)
        {
            if (_view.WindowState == WindowState.Normal && _view.ToolGrid.Width == 200)
            {
                //_view.WindowState = WindowState.Normal;
                _view.Width = 895 + 50;
                _view.ToolGrid.Width = 250;
                //_view.Activate();
            }
            else
            {
                //_view.WindowState = WindowState.Normal;
                _view.Width = 895;
                _view.ToolGrid.Width = 200;
            }
            SetFocus();
        }

        #endregion

        #region ViewModel page navigation event handlers
        /// <summary>
        /// Page managing event handlers
        /// </summary>
        private void Doc_GotoPrev(object sender, EventArgs e)
        {
            SetFocus();
            throw new NotImplementedException();
        }

        private void Doc_GotoNext(object sender, EventArgs e)
        {
            SetFocus();
            throw new NotImplementedException();
        }

        private void Doc_GotoLast(object sender, EventArgs e)
        {
            SetFocus();
            throw new NotImplementedException();
        }

        private void Doc_GotoFirst(object sender, EventArgs e)
        {
            SetFocus();
            throw new NotImplementedException();
        }
        #endregion

        #region Right user control event handlers
        /// <summary>
        /// Setting the the user control on the right
        /// </summary>
        private void ViewModel_FormatText(object sender, EventArgs e)
        {
            _viewModel.CurrentView = _ftViewModel;
            SetFocus();
        }

        private void ViewModel_EditPage(object sender, EventArgs e)
        {
            _viewModel.CurrentView = _pageViewModel;
            SetFocus();
        }

        private void ViewModel_Insert(object sender, EventArgs e)
        {
            _insViewModel = new InsertViewModel(_model);
            _viewModel.CurrentView = _insViewModel;
            SetFocus();
        }

        private void ViewModel_Dict(object sender, EventArgs e)
        {

            _viewModel.CurrentView = _dictViewModel;
            SetFocus();

            //create the dictionary file if it does not exist
            if (_parser.DictFileName == null)
            {
                _createJsonDialog = new CreateFileDialog
                {
                    DataContext = _dictViewModel
                };
                _createJsonDialog.ShowDialog();
            }
        }

        private void SetJsonFileName(object sender, EventArgs e)
        {
            if(_dictViewModel.FileName != null) { 
                _dictViewModel.FileName.Trim();
                if (_dictViewModel.FileName != "")
                {
                    _parser.DictFileName = _dictViewModel.FileName + ".json";
                    System.Diagnostics.Debug.WriteLine(_parser.DictFileName);
                    _createJsonDialog.Close();
                }
            }
            else
            {
                MessageBox.Show("Adjon meg egy fájlnevet!", "Szótár létrehozás hiba", MessageBoxButton.OK, MessageBoxImage.Warning);

            }
        }

        #endregion

        #region Dialog event handlers
        /// <summary>
        /// Event handler to open the color picker
        /// </summary>
        private void openColorDialog(object sender, EventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //elmenteni egy változóba, amit rárakok a kijelölt szövegre
                int col = colorDialog.Color.ToArgb();
                string hexcode = string.Format("{0:x6}", col);

                _ftViewModel.STextColor = hexcode;

                if (_model.SelectionAndFormat == null)
                {
                    _model.SelectionAndFormat = new SelectionAndFormat(_model.select, _model.Align, null);
                }
                _model.SelectionAndFormat.ChangeColor(_ftViewModel.STextColor);
                Console.WriteLine();
                SetSelectionColor();
            }
            SetFocus();
        }

        /// <summary>
        /// Event handler to open the dialog for formatting
        /// </summary>
        private void openMoreFormatting(object sender, EventArgs e)
        {
            FontListWindow _window = new FontListWindow
            {
                DataContext = _ftViewModel
            };
            _window.ShowDialog();
        }

        /// <summary>
        /// Event handler to open the dialog for spacing
        /// </summary>
        private void openMoreSpacing(object sender, EventArgs e)
        {
            Spacing _window = new Spacing
            {
                DataContext = _ftViewModel
            };
            _window.ShowDialog();
        }

        /// <summary>
        /// Event handler to open the dialog for page settings
        /// </summary>
        private void openMorePageSettings(object sender, EventArgs e)
        {
            _psWindow = new PageSettingsWindow
            {
                DataContext = _pageViewModel
            };
            _psWindow.ShowDialog();
        }

        /// <summary>
        /// Event handler to open the pdf viewer with the dictionary help
        /// </summary>
        private void ViewModel_OpenDictHelp(object sender, EventArgs e)
        {
            _dictHelp = new PdfViewer
            {
                DataContext = _viewModel
            };
            _dictHelp.Title = "Dictionary help";
            _dictHelp.ShowDialog();
        }

        #endregion

        #region Menu event handlers
        /// <summary>
        /// Event handler to deleting the selected text
        /// </summary>
        private void ViewModel_DeleteSelection(object sender, EventArgs e)
        {
            SetFocus();
            _view.DocPaper.Selection.Text = "";
        }

        /// <summary>
        /// Event handler for selecting all text
        /// </summary>
        private void ViewModel_SelectAllText(object sender, EventArgs e)
        {
            SetFocus();
            _view.DocPaper.SelectAll();
        }


        private void ViewModel_NewPlainDocument(object sender, EventArgs e)
        {
            SetFocus();
            _model = new DocEditorModel();
            _parser = new ParserMain();
            NewEmpty();
        }

        private void OpenNewDictFile(object sender, EventArgs e)
        {
            //json fájlbetöltős dialog ablak
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Json fájlok (*.json)|*.json";
            if (openFileDialog.ShowDialog() == true)
            {
                //parser json to dict
                _parser.jsonToDict(openFileDialog.FileName);
            }
            _dictViewModel.updateDictList();

        }


        /// <summary>
        /// Event handler for copying text
        /// </summary>
        private void ViewModel_Copy(object sender, ExecutedRoutedEventArgs e)
        {
            SetFocus();
            using (var stream = new MemoryStream())
            {
                _view.DocPaper.Selection.Save(stream, DataFormats.Rtf);
                stream.Position = 0;
                //gembox document
                //DocumentModel.Load(stream, LoadOptions.RtfDefault).Content.SaveToClipboard();
            }
        }

        /// <summary>
        /// Event handler for pasting text
        /// </summary>
        private void ViewModel_Paste(object sender, ExecutedRoutedEventArgs e)
        {
            SetFocus();
            using (var stream = new MemoryStream())
            {
                // Save RichTextBox content to RTF stream.
                var textRange = new TextRange(_view.DocPaper.Document.ContentStart, _view.DocPaper.Document.ContentEnd);
                textRange.Save(stream, DataFormats.Rtf);

                stream.Position = 0;

                // Load document from RTF stream and prepend or append clipboard content to it.
                var document = _model.Load(stream);
                //var position = (string)e.Parameter == "prepend" ? document.Content.Start : document.Content.End;
                //position.LoadFromClipboard();

                stream.Position = 0;

                // Save document to RTF stream.
                //document.Save(stream);

                stream.Position = 0;

                // Load RTF stream into RichTextBox.
                textRange.Load(stream, DataFormats.Rtf);
            }

        }

        /// <summary>
        /// Event handler for cutting text
        /// </summary>
        private void ViewModel_Cut(object sender, ExecutedRoutedEventArgs e)
        {
            SetFocus();
            this.ViewModel_Copy(sender, e);

            _view.DocPaper.Selection.Text = string.Empty;

        }
        #endregion

        #region Text alignment event handlers
        /// <summary>
        /// Center alignment for the paragraph
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetCenterAlignment(object sender, EventArgs e)
        {
            SetFocus();
            if (_model.SelectionAndFormat != null && _model.SelectionAndFormat.Align != Alignment.Center)
            {
                _model.SelectionAndFormat.SetCenterAlignment();
                _view.DocPaper.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Center);
            }
            else if (_model.select.SelectedString != null && _model.SelectionAndFormat == null)
            {
                _model.SelectionAndFormat = new SelectionAndFormat(_model.select, _model.Align, null);

                _model.SelectionAndFormat.SetCenterAlignment();

                //get the current paragraph
                _view.DocPaper.CaretPosition.Paragraph.TextAlignment = TextAlignment.Center;
                
                //_view.DocPaper.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Center);
            }
            else
            {
                _model.Align = Alignment.Center;
                _view.DocPaper.CaretPosition.Paragraph.TextAlignment = TextAlignment.Center;
                //_view.DocPaper.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Center);
            }
        }

        /// <summary>
        /// Right alignment for the paragraph
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetRightAlignment(object sender, EventArgs e)
        {
            SetFocus();
            if (_model.SelectionAndFormat != null && _model.SelectionAndFormat.Align != Alignment.Right)
            {
                _model.SelectionAndFormat.SetRightAlignment();
                //Paragraph par = ;
                //_view.DocPaper.Selection.Start.Paragraph.TextAlignment = TextAlignment.Right;

                //par.ApplyPropertyValue(Paragraph.TextAlignmentProperty, value: _model.SelectionAndFormat.Align);
                //_view.DocPaper.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, value: TextAlignment.Right);
                _view.DocPaper.CaretPosition.Paragraph.TextAlignment = TextAlignment.Right;

            }
            else if (_model.select.SelectedString != null && _model.SelectionAndFormat == null)
            {
                _model.SelectionAndFormat = new SelectionAndFormat(_model.select, _model.Align, null);

                _model.SelectionAndFormat.SetRightAlignment();
                //aragraph par = _view.DocPaper.Selection.Start.Paragraph;
                //_view.DocPaper.Selection.Start.Paragraph.TextAlignment = TextAlignment.Right;
                //_view.DocPaper.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, value: TextAlignment.Right);
                _view.DocPaper.CaretPosition.Paragraph.TextAlignment = TextAlignment.Right;

            }
            else
            {
                _model.Align = Alignment.Right;
                _view.DocPaper.CaretPosition.Paragraph.TextAlignment = TextAlignment.Right;

            }
        }

        /// <summary>
        /// Justify alignment for the paragraph
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetJustifyAlignment(object sender, EventArgs e)
        {
            SetFocus();
            if (_model.SelectionAndFormat != null && _model.SelectionAndFormat.Align != Alignment.Justify)
            {
                _model.SelectionAndFormat.SetJustifyAlignment();
                //view.DocPaper.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, value: TextAlignment.Justify);
                _view.DocPaper.CaretPosition.Paragraph.TextAlignment = TextAlignment.Justify;

            }
            else if (_model.select.SelectedString != null && _model.SelectionAndFormat == null)
            {
                _model.SelectionAndFormat = new SelectionAndFormat(_model.select, _model.Align, null);

                _model.SelectionAndFormat.SetJustifyAlignment();
                //_view.DocPaper.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, value: TextAlignment.Justify);
                _view.DocPaper.CaretPosition.Paragraph.TextAlignment = TextAlignment.Justify;

            }
            else
            {
                _model.Align = Alignment.Justify;
                _view.DocPaper.CaretPosition.Paragraph.TextAlignment = TextAlignment.Justify;

            }
        }


        /// <summary>
        /// Left alignment for the paragraph
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetLeftAlignment(object sender, EventArgs e)
        {
            SetFocus();
            if (_model.SelectionAndFormat != null && _model.SelectionAndFormat.Align != Alignment.Left)
            {
                _model.SelectionAndFormat.SetLeftAlignment();
                //_view.DocPaper.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Left);
                _view.DocPaper.CaretPosition.Paragraph.TextAlignment = TextAlignment.Left;

            }
            else if (_model.select.SelectedString != null && _model.SelectionAndFormat == null)
            {
                _model.SelectionAndFormat = new SelectionAndFormat(_model.select, _model.Align, null);

                _model.SelectionAndFormat.SetLeftAlignment();
                //_view.DocPaper.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Left);
                _view.DocPaper.CaretPosition.Paragraph.TextAlignment = TextAlignment.Left;

            }
            else
            {
                _model.Align = Alignment.Left;
                _view.DocPaper.CaretPosition.Paragraph.TextAlignment = TextAlignment.Left;
            }
        }


        #endregion

        #region Text formatting event handlers
        /// <summary>
        /// Delete all added formatting and revert to the default formatting
        /// </summary>
        private void DeleteFormatting(object sender, EventArgs e)
        {
            SetFocus();
            _model.SelectionAndFormat.DeleteAllFormatting();
            SetStyle(_model.SelectionAndFormat.Formatting);
            _view.DocPaper.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, value: "");
            _view.DocPaper.Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Center);
        }

        /// <summary>
        /// Use one of the formatting syles from the style list
        /// </summary>
        private void Selection_AddFormatStyle(object sender, EventArgs e)
        {
            SetFocus();
            _model.SelectionAndFormat = new SelectionAndFormat(new Selection(selectionOffsets.Start, selectionOffsets.End, _view.DocPaper.Selection.Text), _model.Align, null);
            FormatModel fm = new FormatModel();
            _model.FontStyles.TryGetValue(_ftViewModel.SelectedStyle, out fm);
            SetStyle(fm);
        }

        /// <summary>
        /// Adding new formatting style to the style list by the selected text
        /// </summary>
        private void AddNewStyle(object sender, EventArgs e)
        {
            SetFocus();
            //get the formatting infos from the selected item
            FormatModel[] fm = new FormatModel[_view.DocPaper.Selection.Text.Length];
            Stwf st = new Stwf(new Selection(selectionOffsets.Start, selectionOffsets.End, _view.DocPaper.Selection.Text), fm);

            TextPointer contentStart = _view.DocPaper.Document.ContentStart;

            getFormatting(contentStart.GetPositionAtOffset(selectionOffsets.Start), contentStart.GetPositionAtOffset(selectionOffsets.End));
            //model selectionandformat done

            _model.GetFormatting();

            _model.AddNewFormatStyle(_model.SelectionAndFormat.Formatting);
            _ftViewModel.UpdateStyleList();
        }

        /// <summary>
        /// Event handler for changing the font family
        /// </summary>
        private void Change_FontFamily(object sender, EventArgs e)
        {
            SetFocus();
            //fontsizecmb leküldi a modellbe, és az alapján állítjuk be itt 
            //FontFamilyCmb.SelectedItem
            //if (_view.FontFamilyCmb.SelectedItem != null)
            if (_model.SelectionAndFormat != null && _ftViewModel.SFontFamily != null)
            {
                _model.SelectionAndFormat.ChangeFont(_ftViewModel.SFontFamily);
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, value: _model.SelectionAndFormat.Formatting.Family);
            }
            else if (_model.select.SelectedString != null && _model.SelectionAndFormat == null)
            {
                _model.SelectionAndFormat = new SelectionAndFormat(_model.select, _model.Align, null);
                _model.SelectionAndFormat.ChangeFont(_ftViewModel.SFontFamily);
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, value: _model.SelectionAndFormat.Formatting.Family);
            }
            else
            {

            }
        }

        /// <summary>
        /// Event handler for changing the font size
        /// </summary>
        private void Change_FontSize(object sender, EventArgs e)
        {
            SetFocus();
            //fontsizecmb leküldi a modellbe, és az alapján állítjuk be itt 
            //FontSizeCmb.Text
            //_view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, _model.FormatText.Size);
            if (_model.SelectionAndFormat != null)
            {
                _model.SelectionAndFormat.ChangeSize(Int32.Parse(_ftViewModel.SFontSize));
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, (double)_model.SelectionAndFormat.Formatting.Size);
            }
            else if (_model.select.SelectedString != null && _model.SelectionAndFormat == null)
            {
                _model.SelectionAndFormat = new SelectionAndFormat(_model.select, _model.Align, null);
                _model.SelectionAndFormat.ChangeSize(Int32.Parse(_ftViewModel.SFontSize));
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, (double)_model.SelectionAndFormat.Formatting.Size);
            }
            else
            {

            }
        }

        /// <summary>
        /// Event handler for richtextbox focus
        /// </summary>
        private void RTB_GetFocusBack(object sender, EventArgs e)
        {
            Keyboard.Focus(_view.DocPaper);
            _view.DocPaper.Selection.Select(_view.DocPaper.Selection.Start, _view.DocPaper.Selection.End);
        }
       
        /// <summary>
        /// Event handler for getting the selection for the model
        /// </summary>
        private void setSelection(object sender, EventArgs e)
        {
            SetFocus();
            TextPointer contentStart = _view.DocPaper.Document.ContentStart;

            selectionOffsets.Start = contentStart.GetOffsetToPosition(_view.DocPaper.Selection.Start);
            selectionOffsets.End = contentStart.GetOffsetToPosition(_view.DocPaper.Selection.End);


            if (_view.DocPaper.Selection.Text != null && (selectionOffsets.Start != selectionOffsets.End))
            {
                _model.select.AddToSelected(_view.DocPaper.Selection.Text, selectionOffsets.Start, selectionOffsets.End);
                Console.Error.WriteLine("model.select: " + _model.select.SelectedString); //KISZEDNI
            }
            else
            {
                //delete selection
                _model.select.DeleteSelection();
                Console.Error.WriteLine("model.select: " + _model.select.SelectedString); //KISZEDNI
            }

            //ha már nincs kijelölve, akkor tötölni a modellből a korábbi kijelölést
            //onnan tudjuk, hogy adott dolog kijelölése történik, hogy vagy a start vagy az end változatlan
        }


        /// <summary>
        /// Event handlers for formatting the selected text
        /// </summary>>
        private void Selection_SetBold(object sender, EventArgs e)
        {
            SetFocus();
            if (_model.SelectionAndFormat != null && _model.SelectionAndFormat.Formatting.Weight != "Bold")
            {
                //setting the format bold
                _model.SelectionAndFormat.SetBold();

                //settting the actual selected text bold on the richtextbox
                //_viewModel.setSelectedWeight();
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, value: _model.SelectionAndFormat.Formatting.Weight);
            }
            else if (_model.select.SelectedString != null && _model.SelectionAndFormat == null)
            {
                _model.SelectionAndFormat = new SelectionAndFormat(_model.select, _model.Align, null);

                //setting the format bold
                _model.SelectionAndFormat.SetBold();

                //settting the actual selected text bold on the richtextbox
                //_viewModel.setSelectedWeight();
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, value: _model.SelectionAndFormat.Formatting.Weight);
            }
            else// if (_view.DocPaper.Selection.GetPropertyValue(TextElement.FontWeightProperty).ToString() == "Bold")
            {
                //a további formázást kell átállítani defaultként a modellben
                if (_model.SelectionAndFormat != null && _model.SelectionAndFormat.Formatting.Weight == "Bold")
                {
                    _model.SelectionAndFormat.DeleteWeight();
                    _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, value: _model.SelectionAndFormat.Formatting.Weight);
                }
                else
                {
                    //a modellben a default formázást kell boldra állítani
                    _model.FormatText.Weight = "Bold";
                    //nem lesz jó, mert a caertet nem tudja eddigre, de ha nem veszítené el a focust sosem, akkor jó lenne
                    TextPointer ptr = _view.DocPaper.CaretPosition;
                    _view.DocPaper.Selection.Select(ptr, ptr);
                    _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, value: _model.FormatText.Weight);
                }
            }
        }

        private void Selection_SetItalic(object sender, EventArgs e)
        {
            SetFocus();
            if (_model.SelectionAndFormat != null && _model.SelectionAndFormat.Formatting.Style != "Italic")
            {
                _model.SelectionAndFormat.SetItalic();

                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, value: _model.SelectionAndFormat.Formatting.Style);
            }
            else if (_model.select.SelectedString != null && _model.SelectionAndFormat == null)
            {
                _model.SelectionAndFormat = new SelectionAndFormat(_model.select, _model.Align, null);

                _model.SelectionAndFormat.SetItalic();
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, value: _model.SelectionAndFormat.Formatting.Style);
            }
            else
            {
                //a további formázást kell átállítani defaultként a modellben
                _model.SelectionAndFormat.DeleteStyle();
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, value: _model.SelectionAndFormat.Formatting.Style);

            }
        }

        private void Selection_SetUnderline(object sender, EventArgs e)
        {
            SetFocus();
            if (_view.DocPaper.Selection.GetPropertyValue(Inline.TextDecorationsProperty) != TextDecorations.Underline)
            {
                //setting the actual selected text underlined on the richtextbox
                _view.DocPaper.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, value: TextDecorations.Underline);
            }
            else
            {
                _view.DocPaper.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, value: "");
            }
        }

        private void Selection_SetStrikeThrough(object sender, EventArgs e)
        {
            SetFocus();
            if (_view.DocPaper.Selection.GetPropertyValue(Inline.TextDecorationsProperty) != TextDecorations.Strikethrough)
            {
                //setting the actual selected text strike through on the richtextbox
                _view.DocPaper.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, value: TextDecorations.Strikethrough);
            }
            else
            {
                _view.DocPaper.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, value: "");
            }
        }

        private void Selection_SetSuperScript(object sender, EventArgs e)
        {
            SetFocus();
            if (_model.SelectionAndFormat != null && _model.SelectionAndFormat.Formatting.CharOffset != 2)
            {
                _model.SelectionAndFormat.SetSuperscript();

                double newSize = (double)_model.SelectionAndFormat.Formatting.Size / 2;
                _view.DocPaper.Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Superscript);
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, value: newSize);

            }
            else if (_model.select.SelectedString != null && _model.SelectionAndFormat == null)
            {
                _model.SelectionAndFormat = new SelectionAndFormat(_model.select, _model.Align, null);

                _model.SelectionAndFormat.SetSuperscript();

                double newSize = (double)_model.SelectionAndFormat.Formatting.Size / 2;
                _view.DocPaper.Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Superscript);
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, value: newSize);

            }
            else
            {
                //a további formázást kell átállítani defaultként a modellben
                _model.SelectionAndFormat.DeleteSubSuperscript();
                _view.DocPaper.Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Center);
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, value: (double)_model.SelectionAndFormat.Formatting.Size);
            }
        }

        private void Selection_SetSubScript(object sender, EventArgs e)
        {
            SetFocus();
            if (_model.SelectionAndFormat != null && _model.SelectionAndFormat.Formatting.CharOffset != -2)
            {
                _model.SelectionAndFormat.SetSubscript();

                double newSize = (double)_model.SelectionAndFormat.Formatting.Size / 2;
                _view.DocPaper.Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Subscript);
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, value: newSize);

            }
            else if (_model.select.SelectedString != null && _model.SelectionAndFormat == null)
            {
                _model.SelectionAndFormat = new SelectionAndFormat(_model.select, _model.Align, null);

                _model.SelectionAndFormat.SetSubscript();

                double newSize = (double)_model.SelectionAndFormat.Formatting.Size / 2;
                _view.DocPaper.Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Subscript);
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, value: newSize);
            }
            else
            {
                //a további formázást kell átállítani defaultként a modellben
                _model.SelectionAndFormat.DeleteSubSuperscript();
                _view.DocPaper.Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Center);
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, value: (double)_model.SelectionAndFormat.Formatting.Size);
            }
        }
        #endregion

        #region Pages event handlers
        private void updateRTBList(object sender, EventArgs e)
        {
            //ha a caretposition az rtb végén van, akkor a listának az utolsó eleme = a docpaperrel, és új lap hozzáad
            //végigiterálni, és összeadni a lineheigthokat, ha +1 az utolsó lineból már kívül lenne, akkor új oldal
            double rtbContentHeight = 0; //nem jó:= _view.DocPaper.Document.PageHeight;
            double actualPageHeight = _view.DocPaper.Height - (_model.Margin.Bottom + _model.Margin.Top);

            //végigmenni a sorokon, összeadni a lineheightot, mindig az elejétől a végéig
            TextPointer contentStart = _view.DocPaper.Document.ContentStart;
            int lineCount = _view.DocPaper.Document.Blocks.Count/2 +1;

            rtbContentHeight = (_ftViewModel.LineHeightProp +  _model.FormatText.Size) * lineCount;

            //contentStart.Paragraph.Inlines.


            //a túllógó sor elejére tenni a catetet a sor beolvasásához: _view.DocPaper.CaretPosition.GetLineStartPosition

             if (rtbContentHeight > actualPageHeight)
            {
                //set the last element of the list to the current page from the view
                DocPapers.Add(_view.DocPaper.Document);
                //open a new page
                AddPage();
            }
        }

        private void AddNewParagraph(object sender, EventArgs e)
        {
            //event: hitting the return key
            SetFocus();

            _view.DocPaper.CaretPosition.InsertParagraphBreak();
            Paragraph newpr = new Paragraph();
            _view.DocPaper.Document.Blocks.Add(newpr);
            _view.DocPaper.CaretPosition = _view.DocPaper.CaretPosition.GetNextInsertionPosition(LogicalDirection.Forward);
        }
        #endregion

        #region Parser event handlers
        /// <summary>
        /// When a new is written on the document paper the parser checks if it can be formatted from the dictionary and appies the formatting on it.
        /// </summary>
        private void AutoFormatting_EventHandler(object sender, EventArgs e)
        {
            //event: hitting the space key
            //1. get the word before the space
            //2. check if it is in the dictionary
            //2.a if isnt -> nothing happens
            //2.b if there is -> add the most frequent formatting to the text

            SetFocus();
            string wordToFormat = "";
            TextPointer contentStart = _view.DocPaper.Document.ContentStart;
            _view.DocPaper.CaretPosition.InsertTextInRun(" ");
            _view.DocPaper.CaretPosition = _view.DocPaper.CaretPosition.GetNextInsertionPosition(LogicalDirection.Forward);

            TextPointer ptr = _view.DocPaper.CaretPosition.GetNextInsertionPosition(LogicalDirection.Backward);

            if (new TextRange(_view.DocPaper.CaretPosition, ptr).Text == " ")
            {
                selectionOffsets.End = contentStart.GetOffsetToPosition(ptr);

                //read the word to the next whitespace
                while (ptr != contentStart && ptr != null && ptr.GetNextInsertionPosition(LogicalDirection.Backward) != null)
                {
                    TextRange tr = new TextRange(ptr.GetNextInsertionPosition(LogicalDirection.Backward), ptr);
                    if (tr.Text == " ")
                    {
                        break;
                    }
                    wordToFormat += tr.Text;
                    ptr = ptr.GetNextInsertionPosition(LogicalDirection.Backward);
                }

                selectionOffsets.Start = contentStart.GetOffsetToPosition(ptr);

                _model.SelectForParser = new Stwf(new Selection(selectionOffsets.Start, selectionOffsets.End, _model.ReverseWord(wordToFormat)), new FormatModel[wordToFormat.Length]);
                if (_parser.ContainsElement(_model.ReverseWord(wordToFormat)))
                {
                    _model.SelectForParser = _parser.fromDictionary(_model.SelectForParser);
                    //str add style
                    setFormatting(_model.SelectForParser);
                }
                else if (wordToFormat.Length > 3)
                {
                    List<DictClass> lst = _parser.GetEditDistance(_model.SelectForParser);
                    _ecViewModel.updateDictList(lst);
                    if (lst.Count != 0)
                    {
                        _ecDialog.ShowDialog();
                    }
                }
            }
        }

        /// <summary>
        /// Adding new formatted element to de parser's dictionary
        /// </summary>
        private void AddToDictionary_EventHandler(object sender, EventArgs e)
        {
            foreach (var f in _parser.Dict)
            {
                System.Diagnostics.Debug.WriteLine(f.Str);
                System.Diagnostics.Debug.WriteLine(f.Frequency.ToString());
            }
            //the selected text to an stwf instance
            if (_model.select.SelectedString != null)
            {
                //only one word without spaces can be added to the dictionary
                _model.select = _parser.selectionTrim(_model.select);

                //modify the text pointers for accessing the carachters on the rtb
                getSelection();
                TextPointer contentStart = _view.DocPaper.Document.ContentStart;
                TextRange beforeText = new TextRange(contentStart.GetPositionAtOffset(selectionOffsets.Start - 1), contentStart.GetPositionAtOffset(selectionOffsets.Start));
                TextRange afterText = new TextRange(contentStart, contentStart);
                try
                {
                    afterText = new TextRange(contentStart.GetPositionAtOffset(selectionOffsets.End), contentStart.GetPositionAtOffset(selectionOffsets.End + 1));
                }
                catch (ArgumentNullException)
                {
                    
                }
                _model.select = _parser.selectedTextValidation(_model.select, beforeText.Text, afterText.Text);
                if (_model.select.SelectedString == null)
                {
                    MessageBox.Show("Szótárhoz való hozzáadáshoz jelöljön ki pontosan egy szót!", "Szótár hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    //iterate from start to end to get the formatting information
                    getFormatting(contentStart.GetPositionAtOffset(selectionOffsets.Start), contentStart.GetPositionAtOffset(selectionOffsets.End));
                    
                    //calling the parser algorithms
                    _parser.toDictionary(_model.SelectionAndFormat.SelectedText);
                    _parser.dictToJson();

                    //list update
                    _dictViewModel.updateDictList();

                    //MessageBox.Show("Sikeresen hozzáadva a szótárhoz!", "Szótár", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                //only the selected text can be added to the dictionary error message window
                MessageBox.Show("Szótárhoz hozzáadáshoz jelöljön ki szöveget!", "Szótár hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            SetFocus();
        }

        /// <summary>
        /// Preview for a word's formatting from the dictionary list
        /// </summary>
        private void DictPreview(object sender, EventArgs e)
        {
            if (_dictViewModel.SelectedDictElement != null)
            {
                _dictViewModel.DictString = _dictViewModel.SelectedDictElement.Str;
                Selection sel = new Selection();
                Stwf st = new Stwf(sel, new FormatModel[_dictViewModel.DictString.Length]);

                FormatModel[] fm = _parser.GetFormatting(_dictViewModel.SelectedDictElement);

                _dictViewModel.DisplayText(fm);

            }
            else
            {
                _dictViewModel.DictString = "";
            }
            SetFocus();
        }

        /// <summary>
        /// Delete the seleted element from the dictionary
        /// </summary>
        private void RemoveFromDictionary(object sender, EventArgs e)
        {
            _parser.removeElementFromDictionary(_dictViewModel.SelectedDictElement);
            _dictViewModel.DictString = "";

            //list update
            _dictViewModel.updateDictList();
            _dictViewModel.deletePreview();

            //update json
            _parser.dictToJson();

            SetFocus();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Setting the selection offsets from the model
        /// </summary>
        private void getSelection()
        {
            selectionOffsets.Start = _model.select.StartPointer;
            selectionOffsets.End = _model.select.EndPointer;
        }

        /// <summary>
        /// Getting the formatting of the selected text from the richtextbox
        /// </summary>
        private void getFormatting(TextPointer st, TextPointer nd)
        {
            //iterate through the selected charaters and get all the formatting information for each character
            TextPointer charSt = st;
            TextPointer charNd = st.GetNextInsertionPosition(LogicalDirection.Forward);

            _model.SelectionAndFormat = new SelectionAndFormat(_model.select, _model.Align, null);

            //initialize format array
            _model.SelectionAndFormat.SelectedText.Format = new FormatModel[_model.SelectionAndFormat.SelectedText.SelectedText.SelectedString.Length];
            
            int textp = 0;
            while (charSt != nd && charNd != null && charSt != null && textp < _model.SelectionAndFormat.SelectedText.SelectedText.SelectedString.Length)
            {

                TextRange tr = new TextRange(charSt, charNd);
                {
                    if (tr.Text != string.Empty && (tr.Text.Any(c => char.IsSymbol(c)) || tr.Text.Any(c => char.IsLetterOrDigit(c))))
                    {
                        _model.SelectionAndFormat.SelectedText.Format[textp] = new FormatModel();
                        //add font family
                        object fam = tr.GetPropertyValue(TextElement.FontFamilyProperty);
                        _model.SelectionAndFormat.SelectedText.Format[textp].Family = fam.ToString();

                        //add font style
                        object style = tr.GetPropertyValue(TextElement.FontStyleProperty);
                        _model.SelectionAndFormat.SelectedText.Format[textp].Style = style.ToString();

                        //add font weight
                        object weight = tr.GetPropertyValue(TextElement.FontWeightProperty);
                        _model.SelectionAndFormat.SelectedText.Format[textp].Weight = weight.ToString();

                        //add font size
                        object size = tr.GetPropertyValue(TextElement.FontSizeProperty);
                        _model.SelectionAndFormat.SelectedText.Format[textp].Size = Convert.ToInt32(size);

                        //add font charoffset
                        object offs = tr.GetPropertyValue(Inline.BaselineAlignmentProperty);
                        switch (offs.ToString())
                        {
                            case "Subscript":
                                _model.SelectionAndFormat.SelectedText.Format[textp].CharOffset = -2;
                                break;
                            case "Superscript":
                                _model.SelectionAndFormat.SelectedText.Format[textp].CharOffset = 2;
                                break;
                            default:
                                _model.SelectionAndFormat.SelectedText.Format[textp].CharOffset = 1;
                                break;
                        }

                        //add color
                        object clr = tr.GetPropertyValue(TextElement.ForegroundProperty);
                        _model.SelectionAndFormat.SelectedText.Format[textp].Color = clr.ToString();

                        charSt = charNd;
                    }

                    charNd = charNd.GetNextInsertionPosition(LogicalDirection.Forward);
                    if (charNd == nd)
                    {
                        break;
                    }
                }
                textp++;
            }
        }

        private void setFormatting(Stwf str)
        {
            //fm[0..str.length] -> fm[i] kell az i-edik karakterre rárakni
            //fm[i].style, fm[i].weight, fm[i].family, ...

            //iterate though the rtb from st to nd
            TextPointer contentStart = _view.DocPaper.Document.ContentStart;
            TextPointer st = contentStart.GetPositionAtOffset(str.SelectedText.StartPointer);
            TextPointer nd = contentStart.GetPositionAtOffset(str.SelectedText.EndPointer);

            TextPointer ptr = st;

            int fi = 0;
            while (ptr != nd && ptr != null && ptr.GetNextInsertionPosition(LogicalDirection.Forward) != null && fi < str.SelectedText.SelectedString.Length)
            {
                TextRange tr = new TextRange(ptr, ptr.GetNextInsertionPosition(LogicalDirection.Forward));

                //Style
                tr.ApplyPropertyValue(TextElement.FontStyleProperty, value: str.Format[fi].Style);

                //Weight
                tr.ApplyPropertyValue(TextElement.FontWeightProperty, value: str.Format[fi].Weight);

                //Family
                tr.ApplyPropertyValue(TextElement.FontFamilyProperty, value: str.Format[fi].Family);

                //Size
                tr.ApplyPropertyValue(TextElement.FontSizeProperty, (double)str.Format[fi].Size);

                //BaseLineAlignment
                // Character offset: -2 subscript, 2 superscript, 1 normal
                switch (str.Format[fi].CharOffset)
                {
                    case 1:
                        tr.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Center);
                        break;
                    case -2:
                        tr.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Subscript);
                        break;
                    case 2:
                        tr.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Superscript);
                        break;
                }

                //Color
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, value: str.Format[fi].Color);

                ptr = ptr.GetNextInsertionPosition(LogicalDirection.Forward);
                fi++;
            }
        }

        /// <summary>
        /// Add all formatting to a selection on the document paper
        /// </summary>
        /// <param name="fm">The formatting information</param>
        private void SetStyle(FormatModel fm)
        {
            _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, value: fm.Family);
            _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, value: fm.Style);
            _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, value: fm.Weight);
            _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, (double)fm.Size);
            _view.DocPaper.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, value: fm.Color);
        }

        /// <summary>
        /// Create new empty document
        /// </summary>
        private void NewEmpty()
        {
            DocPapers = new List<FlowDocument>();

            //_viewModel = new MainViewModel(_model);
            _view.DocPaper.Document.Blocks.Clear();
            Paragraph newPr = new Paragraph();
            _view.DocPaper.Document.Blocks.Add(newPr);
            //_viewModel.addFirstPage();

            DocPapers.Add(_view.DocPaper.Document);
            _viewModel.setPageNumbers(1, 1);
        }

        /// <summary>
        /// Add new page to the document
        /// </summary>
        private void AddPage()
        {
            _view.DocPaper.Document.Blocks.Clear();
            //add new empty page
            DocPapers.Add(_view.DocPaper.Document);
            //viewModelben beállítani a jelenlegi oldalt + az összes oldalt
            _viewModel.setPageNumbers(_viewModel.AllPageNumbers + 1, _viewModel.CurrentPageNumber + 1);
        }

        /// <summary>
        /// Setting the focus on the richtextbox
        /// </summary>
        private void SetFocus()
        {
            _view.DocPaper.Focus();
        }

        /// <summary>
        /// Setting the selection color from the color dialog
        /// </summary>
        private void SetSelectionColor()
        {
            _view.DocPaper.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, _model.SelectionAndFormat.Formatting.Color);
        }

        #endregion

    }
}
