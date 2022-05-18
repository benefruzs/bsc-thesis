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
            _parser.dictToJson("test.json");
            _parser.jsonToDict("test.json");

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

            _viewModel.SelectChanged += new EventHandler(setSelection);
            _viewModel.UpdateRTB += new EventHandler(updateRTBList);
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

            _dictViewModel = new DictionaryViewModel(_model, _parser);
            _dictViewModel.AddToDictionary += new EventHandler(AddToDictionary_EventHandler);
            _dictViewModel.SetDictFileName += new EventHandler(SetJsonFileName);
            _dictViewModel.SelectedDictElementChanged += new EventHandler(DictPreview);
            _dictViewModel.RemoveEvent += new EventHandler(RemoveFromDictionary);

            _pageViewModel = new PageSettingsViewModel(_model);
            _pageViewModel.MorePageSettings += new EventHandler(openMorePageSettings);

            //EventBinding.Bind(_view.DocPaper, nameof(_view.DocPaper.SelectionChanged), setSelection);

            DocPapers = new List<FlowDocument>();

            _home = new HomeWindow
            {
                DataContext = _viewModel
            };
            _home.ShowDialog();
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
            if(_view == null)
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
                _home.WindowState = WindowState.Maximized;
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
            newEmpty();
            
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
            if(_view.WindowState == WindowState.Normal)
            {
                _view.WindowState = WindowState.Maximized;
                _view.ToolGrid.Width = 250;
                //_view.Activate();
            }
            else
            {
                _view.WindowState = WindowState.Normal;
                _view.ToolGrid.Width = 200;
            }
            setFocus();
        }

        #endregion

        #region ViewModel page navigation event handlers
        /// <summary>
        /// Page managing event handlers
        /// </summary>
        private void Doc_GotoPrev(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Doc_GotoNext(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Doc_GotoLast(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Doc_GotoFirst(object sender, EventArgs e)
        {
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
            setFocus();
        }

        private void ViewModel_EditPage(object sender, EventArgs e)
        {
            _viewModel.CurrentView = _pageViewModel;
            setFocus();
        }

        private void ViewModel_Insert(object sender, EventArgs e)
        {
            _insViewModel = new InsertViewModel(_model);
            _viewModel.CurrentView = _insViewModel;
            setFocus();
        }

        private void ViewModel_Dict(object sender, EventArgs e)
        {
            
            _viewModel.CurrentView = _dictViewModel;
           
            setFocus();

            //create the dictionary file if it does not exist
            if(_parser.DictFileName == null)
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
            _parser.DictFileName = _dictViewModel.FileName + ".json";
            System.Diagnostics.Debug.WriteLine(_parser.DictFileName);
            _createJsonDialog.Close();
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
                _model.SelectionAndFormat.changeColor(_ftViewModel.STextColor);
                Console.WriteLine();
                setSelectionColor();
            }
            setFocus();
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
            PageSettingsWindow _window = new PageSettingsWindow
            {
                DataContext = _pageViewModel
            };
            _window.ShowDialog();
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
            setFocus();
            _view.DocPaper.Selection.Text = "";
        }

        /// <summary>
        /// Event handler for selecting all text
        /// </summary>
        private void ViewModel_SelectAllText(object sender, EventArgs e)
        {
            setFocus();
            _view.DocPaper.SelectAll();
        }


        private void ViewModel_NewPlainDocument(object sender, EventArgs e)
        {
            setFocus();
            _model = new DocEditorModel();
            newEmpty();
        }


        /// <summary>
        /// Event handler for copying text
        /// </summary>
        private void ViewModel_Copy(object sender, ExecutedRoutedEventArgs e)
        {
            setFocus();
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
            setFocus();
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
            setFocus();
            this.ViewModel_Copy(sender, e);

            _view.DocPaper.Selection.Text = string.Empty;

        }
        #endregion

        #region Text formatting event handlers
        /// <summary>
        /// Event handler for changing the font family
        /// </summary>
        private void Change_FontFamily(object sender, EventArgs e)
        {
            setFocus();
            //fontsizecmb leküldi a modellbe, és az alapján állítjuk be itt 
            //FontFamilyCmb.SelectedItem
            //if (_view.FontFamilyCmb.SelectedItem != null)
            if (_model.SelectionAndFormat != null && _ftViewModel.SFontFamily != null)
            {
                _model.SelectionAndFormat.changeFont(_ftViewModel.SFontFamily);
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, value: _model.SelectionAndFormat.Formatting.Family);
            }
            else if (_model.select.SelectedString != null && _model.SelectionAndFormat == null)
            {
                _model.SelectionAndFormat = new SelectionAndFormat(_model.select, _model.Align, null);
                _model.SelectionAndFormat.changeFont(_ftViewModel.SFontFamily);
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
            setFocus();
            //fontsizecmb leküldi a modellbe, és az alapján állítjuk be itt 
            //FontSizeCmb.Text
            //_view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, _model.FormatText.Size);
            if (_model.SelectionAndFormat != null)
            {
                _model.SelectionAndFormat.changeSize(Int32.Parse(_ftViewModel.SFontSize));
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, (double)_model.SelectionAndFormat.Formatting.Size);
            }
            else if (_model.select.SelectedString != null && _model.SelectionAndFormat == null)
            {
                _model.SelectionAndFormat = new SelectionAndFormat(_model.select, _model.Align, null);
                _model.SelectionAndFormat.changeSize(Int32.Parse(_ftViewModel.SFontSize));
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
        /// Left alignment for the paragraph
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetLeftAlignment(object sender, EventArgs e)
        {
            setFocus();
            if (_model.SelectionAndFormat != null && _model.SelectionAndFormat.Align != Alignment.Left)
            {
                _model.SelectionAndFormat.setLeftAlignment();
                _view.DocPaper.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Left);
            }
            else if (_model.select.SelectedString != null && _model.SelectionAndFormat == null)
            {
                _model.SelectionAndFormat = new SelectionAndFormat(_model.select, _model.Align, null);

                _model.SelectionAndFormat.setLeftAlignment();
                _view.DocPaper.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Left);
            }
            else
            {
                
            }
        }

        /// <summary>
        /// Center alignment for the paragraph
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetCenterAlignment(object sender, EventArgs e)
        {
            setFocus();
            if (_model.SelectionAndFormat != null && _model.SelectionAndFormat.Align != Alignment.Center)
            {
                _model.SelectionAndFormat.setCenterAlignment();
                _view.DocPaper.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Center);
            }
            else if (_model.select.SelectedString != null && _model.SelectionAndFormat == null)
            {
                _model.SelectionAndFormat = new SelectionAndFormat(_model.select, _model.Align, null);

                _model.SelectionAndFormat.setCenterAlignment();
                _view.DocPaper.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Center);
            }
            else
            {
                _view.DocPaper.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Center);
            }
        }

        /// <summary>
        /// Right alignment for the paragraph
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetRightAlignment(object sender, EventArgs e)
        {
            setFocus();
            if (_model.SelectionAndFormat != null && _model.SelectionAndFormat.Align != Alignment.Right)
            {
                _model.SelectionAndFormat.setRightAlignment();
                //Paragraph par = ;
                //_view.DocPaper.Selection.Start.Paragraph.TextAlignment = TextAlignment.Right;

                //par.ApplyPropertyValue(Paragraph.TextAlignmentProperty, value: _model.SelectionAndFormat.Align);
                _view.DocPaper.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, value: TextAlignment.Right);
            }
            else if (_model.select.SelectedString != null && _model.SelectionAndFormat == null)
            {
                _model.SelectionAndFormat = new SelectionAndFormat(_model.select, _model.Align, null);

                _model.SelectionAndFormat.setRightAlignment();
                //aragraph par = _view.DocPaper.Selection.Start.Paragraph;
                //_view.DocPaper.Selection.Start.Paragraph.TextAlignment = TextAlignment.Right;
                _view.DocPaper.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, value: TextAlignment.Right);
            }
            else
            {

            }
        }

        /// <summary>
        /// Justify alignment for the paragraph
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetJustifyAlignment(object sender, EventArgs e)
        {
            setFocus();
            if (_model.SelectionAndFormat != null && _model.SelectionAndFormat.Align != Alignment.Justify)
            {
                _model.SelectionAndFormat.setJustifyAlignment();
                _view.DocPaper.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, value: TextAlignment.Justify);
            }
            else if (_model.select.SelectedString != null && _model.SelectionAndFormat == null)
            {
                _model.SelectionAndFormat = new SelectionAndFormat(_model.select, _model.Align, null);

                _model.SelectionAndFormat.setJustifyAlignment();
                _view.DocPaper.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, value: TextAlignment.Justify);
            }
            else
            {

            }
        }

        /// <summary>
        /// Event handler for getting the selection for the model
        /// </summary>
        private void setSelection(object sender, EventArgs e)
        {
            setFocus();
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
            setFocus();
            if (_model.SelectionAndFormat != null && _model.SelectionAndFormat.Formatting.Weight != "Bold")
            {
                //setting the format bold
                _model.SelectionAndFormat.setBold();

                //settting the actual selected text bold on the richtextbox
                //_viewModel.setSelectedWeight();
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, value: _model.SelectionAndFormat.Formatting.Weight);
            }
            else if(_model.select.SelectedString != null && _model.SelectionAndFormat == null)
            {
                _model.SelectionAndFormat = new SelectionAndFormat(_model.select, _model.Align, null);

                //setting the format bold
                _model.SelectionAndFormat.setBold();

                //settting the actual selected text bold on the richtextbox
                //_viewModel.setSelectedWeight();
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, value:_model.SelectionAndFormat.Formatting.Weight);
            }
            else// if (_view.DocPaper.Selection.GetPropertyValue(TextElement.FontWeightProperty).ToString() == "Bold")
            {
                //a további formázást kell átállítani defaultként a modellben
                if (_model.SelectionAndFormat != null && _model.SelectionAndFormat.Formatting.Weight == "Bold")
                {
                    _model.SelectionAndFormat.deleteWeight();
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
            setFocus();
            if (_model.SelectionAndFormat != null && _model.SelectionAndFormat.Formatting.Style != "Italic")
            {
                _model.SelectionAndFormat.setItalic();

                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, value: _model.SelectionAndFormat.Formatting.Style);
            }
            else if (_model.select.SelectedString != null && _model.SelectionAndFormat == null)
            {
                _model.SelectionAndFormat = new SelectionAndFormat(_model.select, _model.Align, null);

                _model.SelectionAndFormat.setItalic();
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, value: _model.SelectionAndFormat.Formatting.Style);
            }
            else
            {
                //a további formázást kell átállítani defaultként a modellben
                _model.SelectionAndFormat.deleteStyle();
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, value: _model.SelectionAndFormat.Formatting.Style);

            }
        }

        private void Selection_SetUnderline(object sender, EventArgs e)
        {
            setFocus();
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
            setFocus();
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
            setFocus();
            if (_model.SelectionAndFormat != null && _model.SelectionAndFormat.Formatting.CharOffset != 2)
            {
                _model.SelectionAndFormat.setSuperscript();

                double newSize = (double)_model.SelectionAndFormat.Formatting.Size / 2;
                _view.DocPaper.Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Superscript);
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, value: newSize);

            }
            else if (_model.select.SelectedString != null && _model.SelectionAndFormat == null)
            {
                _model.SelectionAndFormat = new SelectionAndFormat(_model.select, _model.Align, null);

                _model.SelectionAndFormat.setSuperscript();

                double newSize = (double)_model.SelectionAndFormat.Formatting.Size / 2;
                _view.DocPaper.Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Superscript);
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, value: newSize);

            }
            else
            {
                //a további formázást kell átállítani defaultként a modellben
                _model.SelectionAndFormat.deleteSubSuperscript();
                _view.DocPaper.Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Center);
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, value: (double)_model.SelectionAndFormat.Formatting.Size);
            }
        }

        private void Selection_SetSubScript(object sender, EventArgs e)
        {
            setFocus();
            if (_model.SelectionAndFormat != null && _model.SelectionAndFormat.Formatting.CharOffset != -2)
            {
                _model.SelectionAndFormat.setSubscript();

                double newSize = (double)_model.SelectionAndFormat.Formatting.Size / 2;
                _view.DocPaper.Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Subscript);
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, value: newSize);

            }
            else if (_model.select.SelectedString != null && _model.SelectionAndFormat == null)
            {
                _model.SelectionAndFormat = new SelectionAndFormat(_model.select, _model.Align, null);

                _model.SelectionAndFormat.setSubscript();

                double newSize = (double)_model.SelectionAndFormat.Formatting.Size / 2;
                _view.DocPaper.Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Subscript);
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, value: newSize);
            }
            else
            {
                //a további formázást kell átállítani defaultként a modellben
                _model.SelectionAndFormat.deleteSubSuperscript();
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
            double rtbContentHeight = _view.DocPaper.Document.PageHeight;
            double actualPageHeight = _view.DocPaper.Height - (_model.Margin.Bottom + _model.Margin.Top);

            if (rtbContentHeight > actualPageHeight)
            {
                //set the last element of the list to the current page from the view
                DocPapers[DocPapers.Count - 1] = _view.DocPaper.Document;
                //open a new page
                addPage();
            }
        }
        #endregion

        #region Parser event handlers
        private void AutoFormatting_EventHandler(object sender, EventArgs e)
        {
            //event: hitting the space button
            //1. get the word before the space
            //2. check if it is in the dictionary
            //2.a if isnt -> nothing happens
            //2.b if there is -> add the most frequent formatting to the text
        }

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
                TextRange beforeText = new TextRange(contentStart.GetPositionAtOffset(selectionOffsets.Start-1), contentStart.GetPositionAtOffset(selectionOffsets.Start));
                TextRange afterText = new TextRange(contentStart.GetPositionAtOffset(selectionOffsets.End), contentStart.GetPositionAtOffset(selectionOffsets.End + 1));
                System.Diagnostics.Debug.WriteLine(beforeText.Text);
                System.Diagnostics.Debug.WriteLine(afterText.Text);
                _model.select = _parser.selectedTextValidation(_model.select, beforeText.Text, afterText.Text);
                if(_model.select.SelectedString == null)
                {
                    MessageBox.Show("Szótárhoz való hozzáadáshoz jelöljön ki pontosan egy szót!", "Szótár hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    //iterate from start to end to get the formatting information
                    getFormatting(contentStart.GetPositionAtOffset(selectionOffsets.Start), contentStart.GetPositionAtOffset(selectionOffsets.End));
                    _parser.toDictionary(_model.SelectionAndFormat.SelectedText);
                    foreach(var f in _parser.Dict)
                    {
                        System.Diagnostics.Debug.WriteLine(f.Str);
                        System.Diagnostics.Debug.WriteLine(f.Frequency.ToString());
                    }
                    for (int i = 0; i < selectionOffsets.End - selectionOffsets.Start - 1; i++)
                    {
                        TextRange ttext = new TextRange(contentStart.GetPositionAtOffset(selectionOffsets.Start + i), contentStart.GetPositionAtOffset(selectionOffsets.Start + i + 1));
                        System.Diagnostics.Debug.WriteLine(ttext.Text);
                    }

                    _parser.dictToJson();

                    //list update
                    _dictViewModel.updateDictList();
                    foreach (var f in _dictViewModel.DictionaryElements)
                    {
                        System.Diagnostics.Debug.WriteLine(f);
                    }

                    //MessageBox.Show("Sikeresen hozzáadva a szótárhoz!", "Szótár", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                //only the selected text can be added to the dictionary error message window
                MessageBox.Show("Szótárhoz hozzáadáshoz jelöljön ki szöveget!", "Szótár hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            setFocus();

            //calling the parser algorithms
        }
        private void DictPreview(object sender, EventArgs e)
        {
            if (_dictViewModel.SelectedDictElement != null)
            {
                _dictViewModel.DictString = _dictViewModel.SelectedDictElement.Str;
                Selection sel = new Selection();
                Stwf st = new Stwf(sel, new FormatModel[_dictViewModel.DictString.Length]);

                FormatModel[] fm = _parser.getFormatting(_dictViewModel.SelectedDictElement);

                //fm[0..str.length] -> fm[i] kell az i-edik karakterre rárakni
                //fm[i].style, fm[i].weight, fm[i].family, ...

                //végig kell iterálni az rtb-ben levő szövegen
                //elejétől az str.lengthig

                //TextPointer contentStart = _dictView.PreviewRtb.Document.ContentStart;
                //TextPointer contentEnd = _dictView.PreviewRtb.Document.ContentEnd;
                //TextRange tr = new TextRange(_dictView.PreviewRtb.Document.ContentStart, _dictView.PreviewRtb.Document.ContentEnd);//_dictView.PreviewRtb.Document.ContentStart.GetNextInsertionPosition(LogicalDirection.Forward));

                //_view.ToolGrid.ContCtrl

                //System.Diagnostics.Debug.WriteLine(tr.Text);

                //tr.ApplyPropertyValue(TextElement.FontFamilyProperty, value: fm[0].Family);
                //tr.ApplyPropertyValue(TextElement.FontStyleProperty, value: fm[0].Style);
                //tr.ApplyPropertyValue(TextElement.ForegroundProperty, value: fm[0].Color);
                _dictViewModel.DisplayText(fm);

            }
            else
            {
                _dictViewModel.DictString = "";
            }
            setFocus();
        }

        private void RemoveFromDictionary(object sender, EventArgs e)
        {
            _parser.removeElementFromDictionary(_dictViewModel.SelectedDictElement);
            _dictViewModel.DictString = "";

            //list update
            _dictViewModel.updateDictList();

            //update json
            _parser.dictToJson();

            setFocus();
        }


        #endregion

        #region Private methods
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
            //végig kell iterálni a kijelölt karaktereken, és onnan kiszedni minden karakterre a formázást
            //selectionOffsets.strarttól endig lekérdezni az adott izét
            //TextPointer selectStart = _view.DocPaper.Selection.Start;
            //TextPointer selectEnd = _view.DocPaper.Selection.End;
            TextPointer charSt = st;
            TextPointer charNd = st.GetNextInsertionPosition(LogicalDirection.Forward);

            _model.SelectionAndFormat = new SelectionAndFormat(_model.select, _model.Align, null);

            //initialize format array
            _model.SelectionAndFormat.SelectedText.Format = new FormatModel[_model.SelectionAndFormat.SelectedText.SelectedText.SelectedString.Length];

            int textp = 0;
            while (charSt != nd && charNd != null && charSt != null)
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

                        //if charoffset -10 vagy +10, akkor a size /2
                        //charoffseteket -2 1 2-re kell átrakni
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


        private void setStyle()
        {
            //kiválaszt egyet a style dictionaryból, aminek a key-nél megegyezik a név, annak a formázását kell alkalmazni rá
            //setstyle művelet, ami paraméterként kapja a dolgokat a formázáshoz
        }


        private void newEmpty()
        {
            _model = new DocEditorModel();
            DocPapers = new List<FlowDocument>();

            //_viewModel = new MainViewModel(_model);
            _view.DocPaper.Document.Blocks.Clear();
            //_viewModel.addFirstPage();

            DocPapers.Add(_view.DocPaper.Document);
            _viewModel.setPageNumbers(1, 1);
        }

        private void addPage()
        {
            _view.DocPaper.Document.Blocks.Clear();
            //add new empty page
            DocPapers.Add(_view.DocPaper.Document);
            //viewModelben beállítani a jelenlegi oldalt + az összes oldalt
            _viewModel.setPageNumbers(_viewModel.AllPageNumbers + 1, _viewModel.CurrentPageNumber + 1);
        }

        private void setFocus()
        {
            _view.DocPaper.Focus();
        }

        private void setSelectionColor()
        {
            _view.DocPaper.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, _model.SelectionAndFormat.Formatting.Color);
        }

        #endregion

    }
}
