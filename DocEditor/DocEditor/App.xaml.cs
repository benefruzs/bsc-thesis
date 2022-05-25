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
using DocEditor.Persistence;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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


        private SelectionOffsets selectionOffsets;

        private CreateFileDialog _createJsonDialog;
        private ErrorCorrectViewModel _ecViewModel;
        private ErrorCorrectDialogWindow _ecDialog;
        private PageSettingsDialog _psWindow;

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

            _viewModel.SelectChanged += new EventHandler(SetSelection);
            _viewModel.AutoFormat += new EventHandler(AutoFormatting_EventHandler);
            _viewModel.NewParagraph += new EventHandler(AddNewParagraph);

            _viewModel.UndoEvent += new EventHandler(UndoLastChange);
            _viewModel.RedoEvent += new EventHandler(RedoLastChange);
            _viewModel.PasteEvent += new EventHandler(ViewModel_Paste);
            _viewModel.CutEvent += new EventHandler(ViewModel_Cut);
            _viewModel.CopyEvent += new EventHandler(ViewModel_Copy);
            _viewModel.ToUpperEvent += new EventHandler(ViewModel_ToUpper);
            _viewModel.ToLowerEvent += new EventHandler(Viewmodel_ToLower);


            _ftViewModel = new FormatTextViewModel(_model);
            _ftViewModel.OpenColorPicker += new EventHandler(OpenColorDialog);

            _ftViewModel.FontFamilyChanged += new EventHandler(Change_FontFamily);
            _ftViewModel.FontSizeChanged += new EventHandler(Change_FontSize);
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
            _pageViewModel.MorePageSettings += new EventHandler(OpenMorePageSettings);
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

            _insViewModel = new InsertViewModel(_model);
            _insViewModel.InsertImageEvent += new EventHandler(ViewModel_InsertImage);

            _home = new HomeWindow
            {
                DataContext = _viewModel
            };
            _home.ShowDialog();
        }

        private void Viewmodel_ToLower(object sender, EventArgs e)
        {
            SetFocus();
            _view.DocPaper.Selection.Text = _view.DocPaper.Selection.Text.ToLower();
            _viewModel.ModelSelect.SelectedString = _viewModel.ModelSelect.SelectedString.ToLower();
        }

        private void ViewModel_ToUpper(object sender, EventArgs e)
        {
            SetFocus();
            _view.DocPaper.Selection.Text = _view.DocPaper.Selection.Text.ToUpper();
            _viewModel.ModelSelect.SelectedString = _viewModel.ModelSelect.SelectedString.ToUpper();
        }

        private void RedoLastChange(object sender, EventArgs e)
        {
            SetFocus();
            _view.DocPaper.Redo();
        }

        private void UndoLastChange(object sender, EventArgs e)
        {
            SetFocus();
            _view.DocPaper.Undo();
        }

        private void ViewModel_InsertImage(object sender, EventArgs e)
        {
            //if there is 
            if(_insViewModel.ImagePath == null || _insViewModel.ImagePath != null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                                         "Portable Network Graphic (*.png)|*.png"
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    //parser json to dict
                    _insViewModel.ImagePath = openFileDialog.FileName;

                    Image newImage = new Image
                    {
                        Source = new BitmapImage(new Uri(_insViewModel.ImagePath)),
                        Margin = new Thickness(10)
                    };

                    if (_insViewModel.ImageWidth != 0 && _insViewModel.ImageHeight != 0)
                    {
                        newImage.Width = _insViewModel.ImageWidth;
                        newImage.Height = _insViewModel.ImageHeight; 
                    }
                    else
                    {
                        newImage.Width = _viewModel.PageWidth - _pageViewModel.Model_GetLeftMargin() - _pageViewModel.Model_GetRightMargin() - 20;
                    }

                    _view.DocPaper.CaretPosition.InsertParagraphBreak();
                    Paragraph newpr = new Paragraph();
                    InlineUIContainer ic = new InlineUIContainer();
                    _view.DocPaper.Document.Blocks.Add(newpr);
                    _view.DocPaper.CaretPosition = _view.DocPaper.CaretPosition.GetNextInsertionPosition(LogicalDirection.Forward);

                    ic.Child = newImage;

                    _view.DocPaper.CaretPosition.Paragraph.Inlines.Add(ic);

                    _viewModel.SetInsertedImage(_view.DocPaper.Document.ContentStart.GetOffsetToPosition(_view.DocPaper.CaretPosition), _insViewModel.ImagePath, 1, 1);
                    _viewModel.AddToImageList();
                }
            }  
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
            TextRange tr = new TextRange(_view.DocPaper.Document.ContentStart.GetPositionAtOffset(_viewModel.ModelSelectForParser.SelectedText.StartPointer), _view.DocPaper.Document.ContentStart.GetPositionAtOffset(_viewModel.ModelSelectForParser.SelectedText.EndPointer));
            tr.Text = "";

            TextPointer tp = _view.DocPaper.Document.ContentStart.GetPositionAtOffset(_viewModel.ModelSelectForParser.SelectedText.StartPointer);
            _view.DocPaper.CaretPosition = tp;

            FormatModel[] fm = _parser.GetFormatting(_ecViewModel.SelectedPossibleDictElement);

            Selection newSel = _parser.GetPossibleFormatting(_ecViewModel.SelectedPossibleDictElement, _viewModel.ModelSelectForParser);
            _viewModel.ModelSelectForParser = new Stwf(newSel, fm);

            _view.DocPaper.CaretPosition.InsertTextInRun(_viewModel.ModelSelectForParser.SelectedText.SelectedString);

            tp = _view.DocPaper.Document.ContentStart.GetPositionAtOffset(_viewModel.ModelSelectForParser.SelectedText.EndPointer);
            _view.DocPaper.CaretPosition = tp.GetNextInsertionPosition(LogicalDirection.Forward);

            SetFormatting(_viewModel.ModelSelectForParser);
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
                _dictViewModel.FileName = _dictViewModel.FileName.Trim();
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
        private void OpenColorDialog(object sender, EventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //elmenteni egy változóba, amit rárakok a kijelölt szövegre
                int col = colorDialog.Color.ToArgb();
                string hexcode = string.Format("{0:x6}", col);

                _ftViewModel.STextColor = hexcode;

                if (_viewModel.ModelSelectionAndFormat == null)
                {
                    _viewModel.ModelSelectionAndFormat = new SelectionAndFormat(_viewModel.ModelSelect, _viewModel.ModelAlign, null);
                }
                _viewModel.ModelSelectionAndFormat.ChangeColor(_ftViewModel.STextColor);
                Console.WriteLine();
                SetSelectionColor();
            }
            SetFocus();
        }

        /// <summary>
        /// Event handler to open the dialog for page settings
        /// </summary>
        private void OpenMorePageSettings(object sender, EventArgs e)
        {
            _psWindow = new PageSettingsDialog
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

        private void ViewModel_OpenUserHelp(object sender, EventArgs e)
        {
            //TODO
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
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Json fájlok (*.json)|*.json"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                //parser json to dict
                _parser.JsonToDict(openFileDialog.FileName);
            }
            _dictViewModel.UpdateDictList();

        }


        /// <summary>
        /// Event handler for copying text
        /// </summary>
        private void ViewModel_Copy(object sender, EventArgs e)
        {
            SetFocus();
            _view.DocPaper.Copy();
        }

        /// <summary>
        /// Event handler for pasting text
        /// </summary>
        private void ViewModel_Paste(object sender, EventArgs e)
        {
            SetFocus();
            _view.DocPaper.Paste();
        }

        /// <summary>
        /// Event handler for cutting text
        /// </summary>
        private void ViewModel_Cut(object sender, EventArgs e)
        {
            SetFocus();
            _view.DocPaper.Cut();
            _viewModel.ModelSelect.DeleteSelection();
        }
        #endregion

        #region Page settings and line spacing event handlers
        private void OkPageSettingsEventHandler(object sender, EventArgs e)
        {
            //_model.SetBottomMargin(Double.Parse(_psWindow.bottom.Text));
            _pageViewModel.Model_SetBottomMargin(Double.Parse(_psWindow.bottom.Text));
            //_model.SetTopMargin(Double.Parse(_psWindow.top.Text));
            _pageViewModel.Model_SetTopMargin(Double.Parse(_psWindow.top.Text));
            //_model.SetLeftMargin(Double.Parse(_psWindow.left.Text));
            _pageViewModel.Model_SetLeftMargin(Double.Parse(_psWindow.left.Text));
            //_model.SetRightMargin(Double.Parse(_psWindow.right.Text));
            _pageViewModel.Model_SetRightMargin(Double.Parse(_psWindow.right.Text));

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
            //_model.SetAllMargins(3.5);
            _pageViewModel.Model_SetAllMargins(3.5);
            _viewModel.SetMarginsRTB();
            _pageViewModel.UpdateMargins();
            _psWindow.Close();
            SetFocus();

        }

        private void Set25MarginEventHandler(object sender, EventArgs e)
        {
            //_model.SetAllMargins(2.5);
            _pageViewModel.Model_SetAllMargins(2.5);
            _viewModel.SetMarginsRTB();
            _pageViewModel.UpdateMargins();
            _psWindow.Close();
            SetFocus();

        }

        private void Set15MarginEventHandler(object sender, EventArgs e)
        {
            //_model.SetAllMargins(1.5);
            _pageViewModel.Model_SetAllMargins(1.5);
            _viewModel.SetMarginsRTB();
            _pageViewModel.UpdateMargins();
            _psWindow.Close();
            SetFocus();
        }

        private void Set05MarginEventHandler(object sender, EventArgs e)
        {
            //_model.SetAllMargins(0.5);
            _pageViewModel.Model_SetAllMargins(0.5);
            _viewModel.SetMarginsRTB();
            _pageViewModel.UpdateMargins();
            _psWindow.Close();
            SetFocus();
        }

        private void ChangeRightMargin(object sender, EventArgs e)
        {
            SetFocus();
            //_model.SetRightMargin(_pageViewModel.RightMargin);
            _pageViewModel.Model_SetRightMargin(_pageViewModel.RightMargin);
            _viewModel.SetMarginsRTB();
            _pageViewModel.UpdateMargins();
        }

        private void ChangeLeftMargin(object sender, EventArgs e)
        {
            SetFocus();
            //_model.SetLeftMargin(_pageViewModel.LeftMargin);
            _pageViewModel.Model_SetLeftMargin(_pageViewModel.LeftMargin);
            _viewModel.SetMarginsRTB();
            _pageViewModel.UpdateMargins();
        }

        private void ChangeTopMargin(object sender, EventArgs e)
        {
            SetFocus();
            //_model.SetTopMargin(_pageViewModel.TopMargin);
            _pageViewModel.Model_SetTopMargin(_pageViewModel.TopMargin);
            _viewModel.SetMarginsRTB();
            _pageViewModel.UpdateMargins();
        }

        private void ChangeBottomMargin(object sender, EventArgs e)
        {
            SetFocus();
            //_model.SetBottomMargin(_pageViewModel.BottomMargin);
            _pageViewModel.Model_SetBottomMargin(_pageViewModel.BottomMargin);
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

        #region Text alignment event handlers
        /// <summary>
        /// Center alignment for the paragraph
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetCenterAlignment(object sender, EventArgs e)
        {
            SetFocus();
            if (_viewModel.ModelSelectionAndFormat != null && _viewModel.ModelSelectionAndFormat.Align != Alignment.Center)
            {
                _viewModel.ModelSelectionAndFormat.SetCenterAlignment();
                _view.DocPaper.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Center);
            }
            else if (_viewModel.ModelSelect.SelectedString != null && _viewModel.ModelSelectionAndFormat == null)
            {
                _viewModel.ModelSelectionAndFormat = new SelectionAndFormat(_viewModel.ModelSelect, _viewModel.ModelAlign, null);

                _viewModel.ModelSelectionAndFormat.SetCenterAlignment();

                //get the current paragraph
                _view.DocPaper.CaretPosition.Paragraph.TextAlignment = TextAlignment.Center;
                
                //_view.DocPaper.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Center);
            }
            else
            {
                _viewModel.ModelAlign = Alignment.Center;
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
            if (_viewModel.ModelSelectionAndFormat != null && _viewModel.ModelSelectionAndFormat.Align != Alignment.Right)
            {
                _viewModel.ModelSelectionAndFormat.SetRightAlignment();
                //Paragraph par = ;
                //_view.DocPaper.Selection.Start.Paragraph.TextAlignment = TextAlignment.Right;

                //par.ApplyPropertyValue(Paragraph.TextAlignmentProperty, value: _viewModel.ModelSelectionAndFormat.Align);
                //_view.DocPaper.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, value: TextAlignment.Right);
                _view.DocPaper.CaretPosition.Paragraph.TextAlignment = TextAlignment.Right;

            }
            else if (_viewModel.ModelSelect.SelectedString != null && _viewModel.ModelSelectionAndFormat == null)
            {
                _viewModel.ModelSelectionAndFormat = new SelectionAndFormat(_viewModel.ModelSelect, _viewModel.ModelAlign, null);

                _viewModel.ModelSelectionAndFormat.SetRightAlignment();
                //aragraph par = _view.DocPaper.Selection.Start.Paragraph;
                //_view.DocPaper.Selection.Start.Paragraph.TextAlignment = TextAlignment.Right;
                //_view.DocPaper.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, value: TextAlignment.Right);
                _view.DocPaper.CaretPosition.Paragraph.TextAlignment = TextAlignment.Right;

            }
            else
            {
                _viewModel.ModelAlign = Alignment.Right;
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
            if (_viewModel.ModelSelectionAndFormat != null && _viewModel.ModelSelectionAndFormat.Align != Alignment.Justify)
            {
                _viewModel.ModelSelectionAndFormat.SetJustifyAlignment();
                //view.DocPaper.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, value: TextAlignment.Justify);
                _view.DocPaper.CaretPosition.Paragraph.TextAlignment = TextAlignment.Justify;

            }
            else if (_viewModel.ModelSelect.SelectedString != null && _viewModel.ModelSelectionAndFormat == null)
            {
                _viewModel.ModelSelectionAndFormat = new SelectionAndFormat(_viewModel.ModelSelect, _viewModel.ModelAlign, null);

                _viewModel.ModelSelectionAndFormat.SetJustifyAlignment();
                //_view.DocPaper.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, value: TextAlignment.Justify);
                _view.DocPaper.CaretPosition.Paragraph.TextAlignment = TextAlignment.Justify;

            }
            else
            {
                _viewModel.ModelAlign = Alignment.Justify;
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
            if (_viewModel.ModelSelectionAndFormat != null && _viewModel.ModelSelectionAndFormat.Align != Alignment.Left)
            {
                _viewModel.ModelSelectionAndFormat.SetLeftAlignment();
                //_view.DocPaper.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Left);
                _view.DocPaper.CaretPosition.Paragraph.TextAlignment = TextAlignment.Left;

            }
            else if (_viewModel.ModelSelect.SelectedString != null && _viewModel.ModelSelectionAndFormat == null)
            {
                _viewModel.ModelSelectionAndFormat = new SelectionAndFormat(_viewModel.ModelSelect, _viewModel.ModelAlign, null);

                _viewModel.ModelSelectionAndFormat.SetLeftAlignment();
                //_view.DocPaper.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Left);
                _view.DocPaper.CaretPosition.Paragraph.TextAlignment = TextAlignment.Left;

            }
            else
            {
                _viewModel.ModelAlign = Alignment.Left;
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
            _viewModel.ModelSelectionAndFormat.DeleteAllFormatting();
            SetStyle(_viewModel.ModelSelectionAndFormat.Formatting);
            _view.DocPaper.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, value: "");
            _view.DocPaper.Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Center);
        }

        /// <summary>
        /// Use one of the formatting syles from the style list
        /// </summary>
        private void Selection_AddFormatStyle(object sender, EventArgs e)
        {
            SetFocus();
            _viewModel.ModelSelectionAndFormat = new SelectionAndFormat(new Selection(selectionOffsets.Start, selectionOffsets.End, _view.DocPaper.Selection.Text), _viewModel.ModelAlign, null);
            FormatModel fm = new FormatModel();
            if (_model.FontStyles.TryGetValue(_ftViewModel.SelectedStyle, out fm))
            {
                SetStyle(fm);
            }
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

            GetFormatting(contentStart.GetPositionAtOffset(selectionOffsets.Start), contentStart.GetPositionAtOffset(selectionOffsets.End));
            //model selectionandformat done

            //_model.GetFormatting();
            _viewModel.Model_GetFormatting();

            //_model.AddNewFormatStyle(_viewModel.ModelSelectionAndFormat.Formatting);
            _viewModel.Model_AddNewFormatStyle();
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
            if (_viewModel.ModelSelectionAndFormat != null && _ftViewModel.SFontFamily != null)
            {
                _viewModel.ModelSelectionAndFormat.ChangeFont(_ftViewModel.SFontFamily);
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, value: _viewModel.ModelSelectionAndFormat.Formatting.Family);
            }
            else if (_viewModel.ModelSelect.SelectedString != null && _viewModel.ModelSelectionAndFormat == null)
            {
                _viewModel.ModelSelectionAndFormat = new SelectionAndFormat(_viewModel.ModelSelect, _viewModel.ModelAlign, null);
                _viewModel.ModelSelectionAndFormat.ChangeFont(_ftViewModel.SFontFamily);
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, value: _viewModel.ModelSelectionAndFormat.Formatting.Family);
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
            //_view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, _viewModel.ModelFormatText.Size);
            if (_viewModel.ModelSelectionAndFormat != null)
            {
                _viewModel.ModelSelectionAndFormat.ChangeSize(Int32.Parse(_ftViewModel.SFontSize));
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, (double)_viewModel.ModelSelectionAndFormat.Formatting.Size);
            }
            else if (_viewModel.ModelSelect.SelectedString != null && _viewModel.ModelSelectionAndFormat == null)
            {
                _viewModel.ModelSelectionAndFormat = new SelectionAndFormat(_viewModel.ModelSelect, _viewModel.ModelAlign, null);
                _viewModel.ModelSelectionAndFormat.ChangeSize(Int32.Parse(_ftViewModel.SFontSize));
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, (double)_viewModel.ModelSelectionAndFormat.Formatting.Size);
            }
            else
            {

            }
        }


       
        /// <summary>
        /// Event handler for getting the selection for the model
        /// </summary>
        private void SetSelection(object sender, EventArgs e)
        {
            SetFocus();
            TextPointer contentStart = _view.DocPaper.Document.ContentStart;

            selectionOffsets.Start = contentStart.GetOffsetToPosition(_view.DocPaper.Selection.Start);
            selectionOffsets.End = contentStart.GetOffsetToPosition(_view.DocPaper.Selection.End);


            if (_view.DocPaper.Selection.Text != null && (selectionOffsets.Start != selectionOffsets.End))
            {
                _viewModel.ModelSelect.AddToSelected(_view.DocPaper.Selection.Text, selectionOffsets.Start, selectionOffsets.End);
                Console.Error.WriteLine("model.select: " + _viewModel.ModelSelect.SelectedString); //KISZEDNI
            }
            else
            {
                //delete selection
                _viewModel.ModelSelect.DeleteSelection();
                Console.Error.WriteLine("model.select: " + _viewModel.ModelSelect.SelectedString); //KISZEDNI
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
            if (_viewModel.ModelSelectionAndFormat != null && _viewModel.ModelSelectionAndFormat.Formatting.Weight != "Bold")
            {
                //setting the format bold
                _viewModel.ModelSelectionAndFormat.SetBold();

                //settting the actual selected text bold on the richtextbox
                //_viewModel.setSelectedWeight();
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, value: _viewModel.ModelSelectionAndFormat.Formatting.Weight);
            }
            else if (_viewModel.ModelSelect.SelectedString != null && _viewModel.ModelSelectionAndFormat == null)
            {
                _viewModel.ModelSelectionAndFormat = new SelectionAndFormat(_viewModel.ModelSelect, _viewModel.ModelAlign, null);

                //setting the format bold
                _viewModel.ModelSelectionAndFormat.SetBold();

                //settting the actual selected text bold on the richtextbox
                //_viewModel.setSelectedWeight();
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, value: _viewModel.ModelSelectionAndFormat.Formatting.Weight);
            }
            else// if (_view.DocPaper.Selection.GetPropertyValue(TextElement.FontWeightProperty).ToString() == "Bold")
            {
                //a további formázást kell átállítani defaultként a modellben
                if (_viewModel.ModelSelectionAndFormat != null && _viewModel.ModelSelectionAndFormat.Formatting.Weight == "Bold")
                {
                    _viewModel.ModelSelectionAndFormat.DeleteWeight();
                    _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, value: _viewModel.ModelSelectionAndFormat.Formatting.Weight);
                }
                else
                {
                    //a modellben a default formázást kell boldra állítani
                    _viewModel.ModelFormatText.Weight = "Bold";
                    //nem lesz jó, mert a caertet nem tudja eddigre, de ha nem veszítené el a focust sosem, akkor jó lenne
                    TextPointer ptr = _view.DocPaper.CaretPosition;
                    _view.DocPaper.Selection.Select(ptr, ptr);
                    _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, value: _viewModel.ModelFormatText.Weight);
                }
            }
        }

        private void Selection_SetItalic(object sender, EventArgs e)
        {
            SetFocus();
            if (_viewModel.ModelSelectionAndFormat != null && _viewModel.ModelSelectionAndFormat.Formatting.Style != "Italic")
            {
                _viewModel.ModelSelectionAndFormat.SetItalic();

                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, value: _viewModel.ModelSelectionAndFormat.Formatting.Style);
            }
            else if (_viewModel.ModelSelect.SelectedString != null && _viewModel.ModelSelectionAndFormat == null)
            {
                _viewModel.ModelSelectionAndFormat = new SelectionAndFormat(_viewModel.ModelSelect, _viewModel.ModelAlign, null);

                _viewModel.ModelSelectionAndFormat.SetItalic();
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, value: _viewModel.ModelSelectionAndFormat.Formatting.Style);
            }
            else
            {
                //a további formázást kell átállítani defaultként a modellben
                _viewModel.ModelSelectionAndFormat.DeleteStyle();
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, value: _viewModel.ModelSelectionAndFormat.Formatting.Style);

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
            if (_viewModel.ModelSelectionAndFormat != null && _viewModel.ModelSelectionAndFormat.Formatting.CharOffset != 2)
            {
                _viewModel.ModelSelectionAndFormat.SetSuperscript();

                double newSize = (double)_viewModel.ModelSelectionAndFormat.Formatting.Size / 2;
                _view.DocPaper.Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Superscript);
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, value: newSize);

            }
            else if (_viewModel.ModelSelect.SelectedString != null && _viewModel.ModelSelectionAndFormat == null)
            {
                _viewModel.ModelSelectionAndFormat = new SelectionAndFormat(_viewModel.ModelSelect, _viewModel.ModelAlign, null);

                _viewModel.ModelSelectionAndFormat.SetSuperscript();

                double newSize = (double)_viewModel.ModelSelectionAndFormat.Formatting.Size / 2;
                _view.DocPaper.Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Superscript);
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, value: newSize);

            }
            else
            {
                //a további formázást kell átállítani defaultként a modellben
                _viewModel.ModelSelectionAndFormat.DeleteSubSuperscript();
                _view.DocPaper.Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Center);
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, value: (double)_viewModel.ModelSelectionAndFormat.Formatting.Size);
            }
        }

        private void Selection_SetSubScript(object sender, EventArgs e)
        {
            SetFocus();
            if (_viewModel.ModelSelectionAndFormat != null && _viewModel.ModelSelectionAndFormat.Formatting.CharOffset != -2)
            {
                _viewModel.ModelSelectionAndFormat.SetSubscript();

                double newSize = (double)_viewModel.ModelSelectionAndFormat.Formatting.Size / 2;
                _view.DocPaper.Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Subscript);
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, value: newSize);

            }
            else if (_viewModel.ModelSelect.SelectedString != null && _viewModel.ModelSelectionAndFormat == null)
            {
                _viewModel.ModelSelectionAndFormat = new SelectionAndFormat(_viewModel.ModelSelect, _viewModel.ModelAlign, null);

                _viewModel.ModelSelectionAndFormat.SetSubscript();

                double newSize = (double)_viewModel.ModelSelectionAndFormat.Formatting.Size / 2;
                _view.DocPaper.Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Subscript);
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, value: newSize);
            }
            else
            {
                //a további formázást kell átállítani defaultként a modellben
                _viewModel.ModelSelectionAndFormat.DeleteSubSuperscript();
                _view.DocPaper.Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Center);
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, value: (double)_viewModel.ModelSelectionAndFormat.Formatting.Size);
            }
        }
        #endregion

        #region Pages event handlers

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

                _viewModel.ModelSelect = new Selection(selectionOffsets.Start, selectionOffsets.End, _viewModel.Model_ReverseWord(wordToFormat));

                GetFormatting(contentStart.GetPositionAtOffset(selectionOffsets.Start), contentStart.GetPositionAtOffset(selectionOffsets.End));

                //calling the parser algorithms
                
                _viewModel.ModelSelectForParser = new Stwf(new Selection(selectionOffsets.Start, selectionOffsets.End, _viewModel.ModelSelectionAndFormat.SelectedText.SelectedText.SelectedString), new FormatModel[wordToFormat.Length]);
               System.Diagnostics.Debug.WriteLine(_viewModel.ModelSelectForParser.SelectedText.SelectedString);
                if (_parser.ContainsElement(_viewModel.Model_ReverseWord(wordToFormat)))
                {
                    _viewModel.ModelSelectForParser = _parser.FromDictionary(_viewModel.ModelSelectForParser);
                    //str add style
                    SetFormatting(_viewModel.ModelSelectForParser);
                }
                else if (wordToFormat.Length > 3)
                {
                    List<DictClass> lst = _parser.GetEditDistance(_viewModel.ModelSelectForParser);
                    _ecViewModel.UpdateDictList(lst);
                    if (lst.Count != 0)
                    {
                        _ecDialog = new ErrorCorrectDialogWindow()
                        {
                            DataContext = _ecViewModel
                        };
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
            if (_viewModel.ModelSelect.SelectedString != null)
            {
                //only one word without spaces can be added to the dictionary
                _viewModel.ModelSelect = _parser.SelectionTrim(_viewModel.ModelSelect);

                //modify the text pointers for accessing the carachters on the rtb
                GetSelection();
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
                _viewModel.ModelSelect = _parser.SelectedTextValidation(_viewModel.ModelSelect, beforeText.Text, afterText.Text);
                if (_viewModel.ModelSelect.SelectedString == null)
                {
                    MessageBox.Show("Szótárhoz való hozzáadáshoz jelöljön ki pontosan egy szót!", "Szótár hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    //iterate from start to end to get the formatting information
                    GetFormatting(contentStart.GetPositionAtOffset(selectionOffsets.Start), contentStart.GetPositionAtOffset(selectionOffsets.End));
                    
                    //calling the parser algorithms
                    _parser.ToDictionary(_viewModel.ModelSelectionAndFormat.SelectedText);
                    _parser.DictToJson();

                    //list update
                    _dictViewModel.UpdateDictList();

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
            _parser.RemoveElementFromDictionary(_dictViewModel.SelectedDictElement);
            _dictViewModel.DictString = "";

            //list update
            _dictViewModel.UpdateDictList();
            _dictViewModel.DeletePreview();

            //update json
            _parser.DictToJson();

            SetFocus();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Setting the selection offsets from the model
        /// </summary>
        private void GetSelection()
        {
            selectionOffsets.Start = _viewModel.ModelSelect.StartPointer;
            selectionOffsets.End = _viewModel.ModelSelect.EndPointer;
        }

        /// <summary>
        /// Getting the formatting of the selected text from the richtextbox
        /// </summary>
        private void GetFormatting(TextPointer st, TextPointer nd)
        {
            //iterate through the selected charaters and get all the formatting information for each character
            TextPointer charSt = st;
            TextPointer charNd = st.GetNextInsertionPosition(LogicalDirection.Forward);

            _viewModel.ModelSelectionAndFormat = new SelectionAndFormat(_viewModel.ModelSelect, _viewModel.ModelAlign, null);

            //initialize format array
            _viewModel.ModelSelectionAndFormat.SelectedText.Format = new FormatModel[_viewModel.ModelSelectionAndFormat.SelectedText.SelectedText.SelectedString.Length];
            
            int textp = 0;
            while (charSt != nd && charNd != null && charSt != null && textp < _viewModel.ModelSelectionAndFormat.SelectedText.SelectedText.SelectedString.Length)
            {

                TextRange tr = new TextRange(charSt, charNd);
                {
                    if (tr.Text != string.Empty && (tr.Text.Any(c => char.IsSymbol(c)) || tr.Text.Any(c => char.IsLetterOrDigit(c))))
                    {
                        _viewModel.ModelSelectionAndFormat.SelectedText.Format[textp] = new FormatModel();
                        //add font family
                        object fam = tr.GetPropertyValue(TextElement.FontFamilyProperty);
                        _viewModel.ModelSelectionAndFormat.SelectedText.Format[textp].Family = fam.ToString();

                        //add font style
                        object style = tr.GetPropertyValue(TextElement.FontStyleProperty);
                        _viewModel.ModelSelectionAndFormat.SelectedText.Format[textp].Style = style.ToString();

                        //add font weight
                        object weight = tr.GetPropertyValue(TextElement.FontWeightProperty);
                        _viewModel.ModelSelectionAndFormat.SelectedText.Format[textp].Weight = weight.ToString();

                        //add font size
                        object size = tr.GetPropertyValue(TextElement.FontSizeProperty);
                        _viewModel.ModelSelectionAndFormat.SelectedText.Format[textp].Size = Convert.ToInt32(size);

                        //add font charoffset
                        object offs = tr.GetPropertyValue(Inline.BaselineAlignmentProperty);
                        _viewModel.ModelSelectionAndFormat.SelectedText.Format[textp].CharOffset = offs.ToString() switch
                        {
                            "Subscript" => -2,
                            "Superscript" => 2,
                            _ => 1,
                        };

                        //add color
                        object clr = tr.GetPropertyValue(TextElement.ForegroundProperty);
                        _viewModel.ModelSelectionAndFormat.SelectedText.Format[textp].Color = clr.ToString();

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

        private void SetFormatting(Stwf str)
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
            //_viewModel = new MainViewModel(_model);
            _view.DocPaper.Document.Blocks.Clear();
            Paragraph newPr = new Paragraph();
            _view.DocPaper.Document.Blocks.Add(newPr);
            //_viewModel.addFirstPage();
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
            _view.DocPaper.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, _viewModel.ModelSelectionAndFormat.Formatting.Color);
        }

        #endregion

    }
}
