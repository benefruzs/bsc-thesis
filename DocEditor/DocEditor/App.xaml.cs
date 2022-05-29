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
using DocEditor.Model;
using DocEditor.ViewModel;
using DocEditor.Parser;
using DocEditor.Persistence;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DocEditor.Views;
using System.Windows.Media;

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
            _model = new DocEditorModel(new DocEditorFileDataAccess());

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
            _viewModel.SaveDictionaryFile += new EventHandler(SaveDictFile);

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
            _viewModel.OpenHelp += new EventHandler(ViewModel_OpenUserHelp);

            _viewModel.OpenFile += new EventHandler(OpenDocedFile);
            _viewModel.SaveFile += new EventHandler(SaveDocedFile);
            _viewModel.SaveAsFile += new EventHandler(SaveAsDocedFile);
            _viewModel.SaveToPdf += new EventHandler(SaveToPdfFile);
            _viewModel.UpdateRTB += new EventHandler(FileChangedEventHandler);
            _viewModel.HomeOpenFile += new EventHandler(Home_OpenDocedFile);
            _viewModel.HomeSaveFile += new EventHandler(Home_SaveDocedFile);
            _viewModel.HomeSaveAsFile += new EventHandler(Home_SaveAsDocedFile);

            _viewModel.NewNoteDictionary += new EventHandler(Home_NewNoteDictionary);
            _viewModel.NewMathDictionary += new EventHandler(Home_NewMathDictionary);
            _viewModel.NewInfDictionary += new EventHandler(Home_NewInfDictionary);


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
        #endregion

        #region Event handlers for saving and loading files

        /// <summary>
        /// Event handler for opening new file with IT dictionary
        /// </summary>
        private void Home_NewInfDictionary(object sender, EventArgs e)
        {
            Home_NewEmptyFile(sender, e);
            _home.Close();
            string curDir = Directory.GetCurrentDirectory();
            int n = curDir.Length - 25;
            string str = curDir.Remove(n, 25); //\bin\Debug\net5.0-windows
            _parser.DictFileName = (String.Format("{0}\\res\\inf.json", str));
            _parser.JsonToDict();
            _dictViewModel.UpdateDictList();
        }

        /// <summary>
        /// Event handler for opening new file with math dictionary
        /// </summary>
        private void Home_NewMathDictionary(object sender, EventArgs e)
        {
            Home_NewEmptyFile(sender, e);
            _home.Close();
            string curDir = Directory.GetCurrentDirectory();
            int n = curDir.Length - 25;
            string str = curDir.Remove(n, 25); //\bin\Debug\net5.0-windows
            _parser.DictFileName = (String.Format("{0}\\res\\math.json", str));
            _parser.JsonToDict();
            _dictViewModel.UpdateDictList();
        }

        /// <summary>
        /// Event handler for opening new file with dictionary for notes
        /// </summary>
        private void Home_NewNoteDictionary(object sender, EventArgs e)
        {
            Home_NewEmptyFile(sender, e);
            _home.Close();
            string curDir = Directory.GetCurrentDirectory();
            int n = curDir.Length - 25;
            string str = curDir.Remove(n, 25); //\bin\Debug\net5.0-windows
            _parser.DictFileName = (String.Format("{0}\\res\\jegyzet-szotar.json", str));
            _parser.JsonToDict();
            _dictViewModel.UpdateDictList();
        }

        /// <summary>
        /// Event handler for saving files from the home window
        /// </summary>
        private void Home_SaveAsDocedFile(object sender, EventArgs e)
        {
            if(_view != null)
            {
                SaveAsDocedFile(sender, e);
                _home.Close();
            }
            else
            {
                MessageBox.Show("Nincs menthető fájl!", "DocEditor", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Event handler for opening files from the home window
        /// </summary>
        private void Home_OpenDocedFile(object sender, EventArgs e)
        {
            if (_view == null)
            {
                Home_NewEmptyFile(sender, e);
            }
            OpenDocedFile(sender, e);
        }

        /// <summary>
        /// Event handler for saving files from the home window
        /// </summary>
        private void Home_SaveDocedFile(object sender, EventArgs e)
        {
            if (_view != null)
            {
                SaveDocedFile(sender, e);
                _home.Close();
            }
            else
            {
                MessageBox.Show("Nincs menthető fájl!", "DocEditor", MessageBoxButton.OK, MessageBoxImage.Error);
            }    
        }

        /// <summary>
        /// Event handler for making unsaved changes
        /// </summary>
        private void FileChangedEventHandler(object sender, EventArgs e)
        {
            _model.SetFalseFileSaved();
        }

        /// <summary>
        /// Event handler for saving dictionary files
        /// </summary>
        private void SaveDictFile(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Json (*.json)|*.json"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                _parser.DictFileName = saveFileDialog.FileName;
                _parser.DictToJson();
            }
        }

        /// <summary>
        /// Event handler for saving document to pdf
        /// </summary>
        private void SaveToPdfFile(object sender, EventArgs e)
        {
            _view.DocPaper.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog() == true)
            {
                //use either one of the below
                pd.PrintVisual(_view.DocPaper as Visual, "printing as visual");
                //pd.PrintDocument((((IDocumentPaginatorSource)_view.DocPaper.Document).DocumentPaginator), "printing as paginator");
            }
        }

        /// <summary>
        /// Event handler for saving files
        /// </summary>
        private async void SaveAsDocedFile(object sender, EventArgs e)
        {
            try 
            {
                //save the dictionary
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Json (*.json)|*.json"
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    _parser.DictFileName = saveFileDialog.FileName;
                    _model.JsonFile = saveFileDialog.FileName;
                    _parser.DictToJson();
                }

                //save the xaml
                SaveFileDialog saveFileDialog1 = new SaveFileDialog
                {
                    Filter = "Document content (*.xaml)|*.xaml"
                };
                if (saveFileDialog1.ShowDialog() == true)
                {
                    _model.RtfFile = saveFileDialog1.FileName;
                    TextRange range = new TextRange(_view.DocPaper.Document.ContentStart, _view.DocPaper.Document.ContentEnd);
                    FileStream fStream = new FileStream(_model.RtfFile, FileMode.Create);
                    range.Save(fStream, DataFormats.XamlPackage);
                    fStream.Close();
                }

                //save the doced file
                SaveFileDialog saveFileDialog2 = new SaveFileDialog
                {
                    Filter = "DocEditor (*.doced)|*.doced"
                };
                if (saveFileDialog2.ShowDialog() == true)
                {
                    _model.FilePath = saveFileDialog2.FileName;
                    _model.GetFileName(saveFileDialog2.FileName);
                    //_viewModel.SetDocumentName();
                    await _model.SaveFileAsync(saveFileDialog2.FileName);
                }
            }
            catch 
            {
                MessageBox.Show("A fájl mentése sikertelen!", "DocEditor", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Event handler for saving files
        /// </summary>
        private async void SaveDocedFile(object sender, EventArgs e)
        {
            try
            {
                if (_model.IsEmpty)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        Filter = "Json (*.json)|*.json"
                    };
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        _parser.DictFileName = saveFileDialog.FileName;
                        _model.JsonFile = saveFileDialog.FileName;
                        _parser.DictToJson();
                    }

                    //save the xaml
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog
                    {
                        Filter = "Document content (*.xaml)|*.xaml"
                    };
                    if (saveFileDialog1.ShowDialog() == true)
                    {
                        _model.RtfFile = saveFileDialog1.FileName;
                        TextRange range = new TextRange(_view.DocPaper.Document.ContentStart, _view.DocPaper.Document.ContentEnd);
                        FileStream fStream = new FileStream(_model.RtfFile, FileMode.Create);
                        range.Save(fStream, DataFormats.XamlPackage);
                        fStream.Close();
                    }

                    //save the doced file
                    SaveFileDialog saveFileDialog2 = new SaveFileDialog
                    {
                        Filter = "DocEditor (*.doced)|*.doced"
                    };
                    if (saveFileDialog2.ShowDialog() == true)
                    {
                        _model.FilePath = saveFileDialog2.FileName;
                        _model.GetFileName(saveFileDialog2.FileName);
                        //_viewModel.SetDocumentName();
                        await _model.SaveFileAsync(saveFileDialog2.FileName);
                    }
                }
                else
                {
                    //csak mentés a fájlnevekbe
                    await _model.SaveFileAsync(_model.FilePath);

                    //rtb save
                    TextRange range = new TextRange(_view.DocPaper.Document.ContentStart, _view.DocPaper.Document.ContentEnd);
                    FileStream fStream = new FileStream(_model.RtfFile, FileMode.Create);
                    range.Save(fStream, DataFormats.XamlPackage);

                    //json save
                    _parser.DictToJson(_model.JsonFile);
                }
            }
            catch
            {
                MessageBox.Show("A fájl mentése sikertelen!", "DocEditor", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        /// <summary>
        /// Event handler for opening files
        /// </summary>
        private async void OpenDocedFile(object sender, EventArgs e)
        {
           try
           {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "DocEditor (*.doced)|*.doced"
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    //modell betöltés
                    _model.FilePath = openFileDialog.FileName;
                    _model.GetFileName(openFileDialog.FileName);
                    await _model.LoadFileAsync(openFileDialog.FileName);
                }

                //rtb load
                TextRange range;
                FileStream fStream;
                if (File.Exists(_model.RtfFile) && File.Exists(_model.JsonFile))
                {
                    range = new TextRange(_view.DocPaper.Document.ContentStart, _view.DocPaper.Document.ContentEnd);
                    fStream = new FileStream(_model.RtfFile, FileMode.OpenOrCreate);
                    range.Load(fStream, DataFormats.XamlPackage);
                    fStream.Close();

                    //json load
                    _parser.DictFileName = _model.JsonFile;
                    _parser.JsonToDict();
                }
                else
                {
                    MessageBox.Show("A szótár vagy a dokumentum fájl nem található!", "DocEditor", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch
            {
                MessageBox.Show("A fájl betöltése sikertelen!", "DocEditor", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Event hadndler for opening a dictionary file
        /// </summary>
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
        #endregion


        #region Event handler for images
        /// <summary>
        /// Event handler for inserting images
        /// </summary>
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
        {if (_ecViewModel.SelectedPossibleDictElement != null)
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
                try
                {
                    _view.DocPaper.CaretPosition = tp.GetNextInsertionPosition(LogicalDirection.Forward);
                }
                catch (ArgumentNullException) { }

                SetFormatting(_viewModel.ModelSelectForParser);
            }
            _ecViewModel.SelectedPossibleDictElement = null;
            _ecDialog.Close();
        }

        private void EcViewModel_NoThanks(object sender, EventArgs e)
        {
            _ecDialog.Close();
        }
        #endregion

        #region ViewModel home window event handlers
        /// <summary>
        /// Event handler for opening the home window
        /// </summary>
        private void ViewModel_OpenHomeWindow(object sender, EventArgs e)
        {
            _home = new HomeWindow
            {
                DataContext = _viewModel
            };
            _home.ShowDialog();
        }

        /// <summary>
        /// Event handler for closing the home window
        /// </summary>
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

        /// <summary>
        /// Event handler for minimizing the home window
        /// </summary>
        private void Home_Min(object sender, EventArgs e)
        {
            _home.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Event handler for maximizing the home window
        /// </summary>
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

        /// <summary>
        /// Event handler for creating a new file
        /// </summary>
        private void Home_NewEmptyFile(object sender, EventArgs e)
        {
            if (!_model.IsFileSaved && _view != null) //modify after the last save or not
            {
                MessageBoxResult result = MessageBox.Show("Kilép mentés nélkül? ", "DocEditor", MessageBoxButton.YesNo, MessageBoxImage.Question);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        _home.Close();
                        NewEmpty();
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
            else
            {
                if (_view == null)
                {
                    _view = new MainWindow
                    {
                        DataContext = _viewModel
                    };
                    _view.Show();
                }
                _home.Close();
                NewEmpty();
            }
        }

        #endregion

        #region ViewModel main window event handlers

        /// <summary>
        /// Main Window event handlers
        /// </summary>
        private void ViewModel_Exit(object sender, EventArgs e)
        {
            if (!_model.IsFileSaved) //modify after the last save or not
            {
                MessageBoxResult result = MessageBox.Show("Kilép mentés nélkül? ", "DocEditor", MessageBoxButton.YesNo, MessageBoxImage.Question);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        Shutdown();
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
            else
            {
                Shutdown();
            }
        }

        /// <summary>
        /// Event handler for minimizing the main window
        /// </summary>
        private void ViewModel_Min(object sender, EventArgs e)
        {
            _view.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Event handler for maximizing the main window
        /// </summary>
        private void ViewModel_Max(object sender, EventArgs e)
        {
            if (_view.WindowState == WindowState.Normal && _view.ToolGrid.Width == 200)
            {
                _view.Width = 895 + 50;
                _view.ToolGrid.Width = 250;
            }
            else
            {
                _view.Width = 895;
                _view.ToolGrid.Width = 200;
            }
            SetFocus();
        }
        #endregion

        #region Right user control event handlers
        /// <summary>
        /// Setting the the user control on the right to format text view
        /// </summary>
        private void ViewModel_FormatText(object sender, EventArgs e)
        {
            _viewModel.CurrentView = _ftViewModel;
            SetFocus();
        }

        /// <summary>
        /// Setting the the user control on the right to edit page view
        /// </summary>
        private void ViewModel_EditPage(object sender, EventArgs e)
        {
            _viewModel.CurrentView = _pageViewModel;
            SetFocus();
        }

        /// <summary>
        /// Setting the the user control on the right to insert view
        /// </summary>
        private void ViewModel_Insert(object sender, EventArgs e)
        {
            
            _viewModel.CurrentView = _insViewModel;
            SetFocus();
        }

        /// <summary>
        /// Setting the the user control on the right to dictionary view
        /// </summary>
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



        #endregion

        #region Dialog event handlers
        /// <summary>
        /// Event handler for new json file
        /// </summary>
        private void SetJsonFileName(object sender, EventArgs e)
        {
            if (_dictViewModel.FileName != null)
            {
                _dictViewModel.FileName = _dictViewModel.FileName.Trim();
                if (_dictViewModel.FileName != "")
                {
                    _parser.DictFileName = _dictViewModel.FileName + ".json";
                    _createJsonDialog.Close();
                }
            }
            else
            {
                MessageBox.Show("Adjon meg egy fájlnevet!", "Szótár létrehozás hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

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
            _dictHelp.Title = "Szótár súgó";
            _dictHelp.ShowDialog();
        }

        /// <summary>
        /// Event handler for opening the user help window
        /// </summary>
        private void ViewModel_OpenUserHelp(object sender, EventArgs e)
        {
            UserHelpViewer _userHelp = new UserHelpViewer
            {
                DataContext = _viewModel
            };
            _userHelp.Title = "Felhasználói súgó";
            _userHelp.ShowDialog();
        }
        #endregion

        #region Menu event handlers
        /// <summary>
        /// Event handler for making a text lowercase
        /// </summary>
        private void Viewmodel_ToLower(object sender, EventArgs e)
        {
            SetFocus();
            _view.DocPaper.Selection.Text = _view.DocPaper.Selection.Text.ToLower();
            _viewModel.ModelSelect.SelectedString = _viewModel.ModelSelect.SelectedString.ToLower();
        }

        /// <summary>
        /// Event handler for making a text uppercase
        /// </summary>
        private void ViewModel_ToUpper(object sender, EventArgs e)
        {
            SetFocus();
            _view.DocPaper.Selection.Text = _view.DocPaper.Selection.Text.ToUpper();
            _viewModel.ModelSelect.SelectedString = _viewModel.ModelSelect.SelectedString.ToUpper();
        }

        /// <summary>
        /// Event handler for redoing the last changes
        /// </summary>
        private void RedoLastChange(object sender, EventArgs e)
        {
            SetFocus();
            _view.DocPaper.Redo();
        }

        /// <summary>
        /// Event handler for undoing the last changes
        /// </summary>
        private void UndoLastChange(object sender, EventArgs e)
        {
            SetFocus();
            _view.DocPaper.Undo();
        }

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

        /// <summary>
        /// Event handler for creating a new empty document
        /// </summary>
        private void ViewModel_NewPlainDocument(object sender, EventArgs e)
        {
            _model = new DocEditorModel(new DocEditorFileDataAccess());
            _parser = new ParserMain();
            NewEmpty();
            SetFocus();
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
        /// <summary>
        /// Opening the page settings window
        /// </summary>
        private void OkPageSettingsEventHandler(object sender, EventArgs e)
        {
            _pageViewModel.Model_SetBottomMargin(Double.Parse(_psWindow.bottom.Text));
            _pageViewModel.Model_SetTopMargin(Double.Parse(_psWindow.top.Text));
            _pageViewModel.Model_SetLeftMargin(Double.Parse(_psWindow.left.Text));
            _pageViewModel.Model_SetRightMargin(Double.Parse(_psWindow.right.Text));

            _viewModel.SetMarginsRTB();
            _pageViewModel.UpdateMargins();
            _psWindow.Close();
            SetFocus();
        }

        /// <summary>
        /// Closing the page settings window
        /// </summary>
        private void ClosePageSettingsEventHandler(object sender, EventArgs e)
        {
            _psWindow.Close();
            SetFocus();
        }

        /// <summary>
        /// Event handler for setting all margins to 3.5
        /// </summary>
        private void Set35MarginEventHandler(object sender, EventArgs e)
        {
            _pageViewModel.Model_SetAllMargins(3.5);
            _viewModel.SetMarginsRTB();
            _pageViewModel.UpdateMargins();
            _psWindow.Close();
            SetFocus();

        }

        /// <summary>
        /// Event handler for setting all margins to 2.5
        /// </summary>
        private void Set25MarginEventHandler(object sender, EventArgs e)
        {
            _pageViewModel.Model_SetAllMargins(2.5);
            _viewModel.SetMarginsRTB();
            _pageViewModel.UpdateMargins();
            _psWindow.Close();
            SetFocus();
        }

        /// <summary>
        /// Event handler for setting all margins to 1.5
        /// </summary>
        private void Set15MarginEventHandler(object sender, EventArgs e)
        {
            _pageViewModel.Model_SetAllMargins(1.5);
            _viewModel.SetMarginsRTB();
            _pageViewModel.UpdateMargins();
            _psWindow.Close();
            SetFocus();
        }

        /// <summary>
        /// Event handler for setting all margins to 0.5
        /// </summary>
        private void Set05MarginEventHandler(object sender, EventArgs e)
        {
            _pageViewModel.Model_SetAllMargins(0.5);
            _viewModel.SetMarginsRTB();
            _pageViewModel.UpdateMargins();
            _psWindow.Close();
            SetFocus();
        }

        /// <summary>
        /// Event handler for changing the right margin
        /// </summary>
        private void ChangeRightMargin(object sender, EventArgs e)
        {
            SetFocus();
            _pageViewModel.Model_SetRightMargin(_pageViewModel.RightMargin);
            _viewModel.SetMarginsRTB();
            _pageViewModel.UpdateMargins();
        }

        /// <summary>
        /// Event handler for changing the left margin
        /// </summary>
        private void ChangeLeftMargin(object sender, EventArgs e)
        {
            SetFocus();
            _pageViewModel.Model_SetLeftMargin(_pageViewModel.LeftMargin);
            _viewModel.SetMarginsRTB();
            _pageViewModel.UpdateMargins();
        }

        /// <summary>
        /// Event handler for changing the top margin
        /// </summary>
        private void ChangeTopMargin(object sender, EventArgs e)
        {
            SetFocus();
            _pageViewModel.Model_SetTopMargin(_pageViewModel.TopMargin);
            _viewModel.SetMarginsRTB();
            _pageViewModel.UpdateMargins();
        }

        /// <summary>
        /// Event handler for changing the bottom margin
        /// </summary>
        private void ChangeBottomMargin(object sender, EventArgs e)
        {
            SetFocus();
            _pageViewModel.Model_SetBottomMargin(_pageViewModel.BottomMargin);
            _viewModel.SetMarginsRTB();
            _pageViewModel.UpdateMargins();
        }

        /// <summary>
        /// Event handler for decreasing the line height
        /// </summary>
        private void ViewModel_LessLineHeight(object sender, EventArgs e)
        {
            _ftViewModel.DecreaseLineHeight();
            _view.DocPaper.Document.LineHeight = _ftViewModel.LineHeightProp;
            SetFocus();
        }

        /// <summary>
        /// Event handler for increasing the line height
        /// </summary>
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
            }
            else
            {
                _viewModel.ModelAlign = Alignment.Center;
                _view.DocPaper.CaretPosition.Paragraph.TextAlignment = TextAlignment.Center;
            }
        }

        /// <summary>
        /// Right alignment for the paragraph
        /// </summary>
        private void SetRightAlignment(object sender, EventArgs e)
        {
            SetFocus();
            if (_viewModel.ModelSelectionAndFormat != null && _viewModel.ModelSelectionAndFormat.Align != Alignment.Right)
            {
                _viewModel.ModelSelectionAndFormat.SetRightAlignment();
               
                _view.DocPaper.CaretPosition.Paragraph.TextAlignment = TextAlignment.Right;

            }
            else if (_viewModel.ModelSelect.SelectedString != null && _viewModel.ModelSelectionAndFormat == null)
            {
                _viewModel.ModelSelectionAndFormat = new SelectionAndFormat(_viewModel.ModelSelect, _viewModel.ModelAlign, null);

                _viewModel.ModelSelectionAndFormat.SetRightAlignment();
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
        private void SetJustifyAlignment(object sender, EventArgs e)
        {
            SetFocus();
            if (_viewModel.ModelSelectionAndFormat != null && _viewModel.ModelSelectionAndFormat.Align != Alignment.Justify)
            {
                _viewModel.ModelSelectionAndFormat.SetJustifyAlignment();
                _view.DocPaper.CaretPosition.Paragraph.TextAlignment = TextAlignment.Justify;

            }
            else if (_viewModel.ModelSelect.SelectedString != null && _viewModel.ModelSelectionAndFormat == null)
            {
                _viewModel.ModelSelectionAndFormat = new SelectionAndFormat(_viewModel.ModelSelect, _viewModel.ModelAlign, null);

                _viewModel.ModelSelectionAndFormat.SetJustifyAlignment();
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
        private void SetLeftAlignment(object sender, EventArgs e)
        {
            SetFocus();
            if (_viewModel.ModelSelectionAndFormat != null && _viewModel.ModelSelectionAndFormat.Align != Alignment.Left)
            {
                _viewModel.ModelSelectionAndFormat.SetLeftAlignment();
                _view.DocPaper.CaretPosition.Paragraph.TextAlignment = TextAlignment.Left;

            }
            else if (_viewModel.ModelSelect.SelectedString != null && _viewModel.ModelSelectionAndFormat == null)
            {
                _viewModel.ModelSelectionAndFormat = new SelectionAndFormat(_viewModel.ModelSelect, _viewModel.ModelAlign, null);

                _viewModel.ModelSelectionAndFormat.SetLeftAlignment();
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
            string styleStr = _view.DocPaper.Selection.Text.Trim(' ', '\r', '\n');
            FormatModel[] fm = new FormatModel[styleStr.Length];
            
            Stwf st = new Stwf(new Selection(selectionOffsets.Start, selectionOffsets.End, styleStr), fm);

            TextPointer contentStart = _view.DocPaper.Document.ContentStart;

            GetFormatting(contentStart.GetPositionAtOffset(selectionOffsets.Start), contentStart.GetPositionAtOffset(selectionOffsets.End));
            //model selectionandformat done

            _viewModel.Model_GetFormatting();

            _viewModel.Model_AddNewFormatStyle();
            _ftViewModel.UpdateStyleList();
        }

        /// <summary>
        /// Event handler for changing the font family
        /// </summary>
        private void Change_FontFamily(object sender, EventArgs e)
        {
            SetFocus();

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
            }
            else
            {
                //delete selection
                _viewModel.ModelSelect.DeleteSelection();
            }
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

                //setting the actual selected text bold on the richtextbox
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, value: _viewModel.ModelSelectionAndFormat.Formatting.Weight);
            }
            else if (_viewModel.ModelSelect.SelectedString != null && _viewModel.ModelSelectionAndFormat == null)
            {
                _viewModel.ModelSelectionAndFormat = new SelectionAndFormat(_viewModel.ModelSelect, _viewModel.ModelAlign, null);

                //setting the format bold
                _viewModel.ModelSelectionAndFormat.SetBold();

                //setting the actual selected text bold on the richtextbox
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, value: _viewModel.ModelSelectionAndFormat.Formatting.Weight);
            }
            else
            {
                if (_viewModel.ModelSelectionAndFormat != null && _viewModel.ModelSelectionAndFormat.Formatting.Weight == "Bold")
                {
                    _viewModel.ModelSelectionAndFormat.DeleteWeight();
                    _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, value: _viewModel.ModelSelectionAndFormat.Formatting.Weight);
                }
                else
                {
                    _viewModel.ModelFormatText.Weight = "Bold";
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
            BaselineAlignment curr = (BaselineAlignment)_view.DocPaper.Selection.GetPropertyValue(Inline.BaselineAlignmentProperty);

            if (_model.SelectionAndFormat != null)
            {
                _viewModel.ModelSelectionAndFormat.Formatting.CharOffset = (curr == BaselineAlignment.Center) ? 1 : ((curr == BaselineAlignment.Subscript) ? -2 : 2);
            }

            SetFocus();
            if (_model.SelectionAndFormat != null && _model.SelectionAndFormat.Formatting.CharOffset != 2)
            {
                _model.SelectionAndFormat.SetSuperscript();
                _model.FormatText.Size = _viewModel.ModelSelectionAndFormat.Formatting.Size;
                double newSize = (double)_model.SelectionAndFormat.Formatting.Size / 2;
                _view.DocPaper.Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Superscript);
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, value: newSize);

            }
            else if (_model.select.SelectedString != null && _model.SelectionAndFormat == null)
            {
                _model.SelectionAndFormat = new SelectionAndFormat(_model.select, _model.Align, null);

                _model.SelectionAndFormat.SetSuperscript();
                _model.FormatText.Size = _viewModel.ModelSelectionAndFormat.Formatting.Size;
                double newSize = (double)_model.SelectionAndFormat.Formatting.Size / 2;
                _view.DocPaper.Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Superscript);
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, value: newSize);
            }
            else
            {
                _model.SelectionAndFormat.DeleteSubSuperscript();
                _view.DocPaper.Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Center);
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, value: (double)_model.FormatText.Size);
            }
        }

        private void Selection_SetSubScript(object sender, EventArgs e)
        {
            SetFocus();
            BaselineAlignment curr = (BaselineAlignment)_view.DocPaper.Selection.GetPropertyValue(Inline.BaselineAlignmentProperty);

            if (_model.SelectionAndFormat != null)
            {
                _viewModel.ModelSelectionAndFormat.Formatting.CharOffset = (curr == BaselineAlignment.Center) ? 1 : ((curr == BaselineAlignment.Subscript) ? -2 : 2);
            }
            if (_model.SelectionAndFormat != null && _model.SelectionAndFormat.Formatting.CharOffset != -2)
            {
                _model.SelectionAndFormat.SetSubscript();
                _model.FormatText.Size = _viewModel.ModelSelectionAndFormat.Formatting.Size;
                double newSize = (double)_model.SelectionAndFormat.Formatting.Size / 2;
                _view.DocPaper.Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Subscript);
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, value: newSize);

            }
            else if (_model.select.SelectedString != null && _model.SelectionAndFormat == null)
            {
                _model.SelectionAndFormat = new SelectionAndFormat(_model.select, _model.Align, null);

                _model.SelectionAndFormat.SetSubscript();
                _model.FormatText.Size = _viewModel.ModelSelectionAndFormat.Formatting.Size;
                double newSize = (double)_model.SelectionAndFormat.Formatting.Size / 2;
                _view.DocPaper.Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Subscript);
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, value: newSize);
            }
            else
            {
                _model.SelectionAndFormat.DeleteSubSuperscript();
                _view.DocPaper.Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Center);
                _view.DocPaper.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, value: (double)_model.FormatText.Size);
            }
        }
        #endregion

        #region Pages event handlers

        private void AddNewParagraph(object sender, EventArgs e)
        {
            //event: hitting the return key
            SetFocus();

            _view.DocPaper.CaretPosition = _view.DocPaper.CaretPosition.InsertParagraphBreak();
        }
        #endregion

        #region Parser event handlers
        /// <summary>
        /// When a new word is written on the document paper after hitting the space key, the parser checks if it can be formatted from the dictionary and applies the formatting on it.
        /// If the dictionary does not contain the word, gets the similar words from the dictionary by their edit distance. 
        /// </summary>
        private void AutoFormatting_EventHandler(object sender, EventArgs e)
        {
            //event: hitting the space key
            //1. get the word before the space
            //2. check if it is in the dictionary
            //2.a if isnt -> checking edit distances
            //2.b if there is -> add the most frequent formatting to the text

            SetFocus();
            string wordToFormat = "";
            TextPointer contentStart = _view.DocPaper.Document.ContentStart;
            _view.DocPaper.CaretPosition.InsertTextInRun(" ");
           
            try
            {
                _view.DocPaper.CaretPosition = _view.DocPaper.CaretPosition.GetNextInsertionPosition(LogicalDirection.Forward);
            }catch(ArgumentNullException)
            {
            }
            

            TextPointer ptr = _view.DocPaper.CaretPosition.GetNextInsertionPosition(LogicalDirection.Backward);
            TextPointer ptrEnd = ptr;

            if (new TextRange(_view.DocPaper.CaretPosition, ptr).Text == " ")
            {
                selectionOffsets.End = contentStart.GetOffsetToPosition(ptr);

                //read the word to the next whitespace
                while (ptr != contentStart && ptr != null && ptr.GetNextInsertionPosition(LogicalDirection.Backward) != null)
                {
                    TextRange tr = new TextRange(ptr.GetNextInsertionPosition(LogicalDirection.Backward), ptr);
                    if (tr.Text == " " || tr.Text == "\r\n")
                    {
                        break;
                    }
                    wordToFormat += tr.Text;
                    ptr = ptr.GetNextInsertionPosition(LogicalDirection.Backward);
                }

                selectionOffsets.Start = contentStart.GetOffsetToPosition(ptr);

                TextRange tmp = new TextRange(ptr, ptrEnd);

                //reverse and trim the word for the formatting
                wordToFormat = _viewModel.Model_ReverseWord(wordToFormat);
                selectionOffsets.Start += tmp.Text.Length - wordToFormat.Length;

                //_viewModel.ModelSelect = new Selection(selectionOffsets.Start, selectionOffsets.End, _viewModel.Model_ReverseWord(wordToFormat));
                _viewModel.ModelSelect = new Selection(selectionOffsets.Start, selectionOffsets.End, wordToFormat);

                //GetFormatting(contentStart.GetPositionAtOffset(selectionOffsets.Start), contentStart.GetPositionAtOffset(selectionOffsets.End));
                GetFormatting(ptr, ptrEnd);

                
                //calling the parser algorithms
                _viewModel.ModelSelectForParser = new Stwf(new Selection(selectionOffsets.Start, selectionOffsets.End, _viewModel.ModelSelectionAndFormat.SelectedText.SelectedText.SelectedString), new FormatModel[wordToFormat.Length]);
                if (_parser.ContainsElement(wordToFormat))
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

            _viewModel.ModelSelect.SelectedString.Trim();
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
                        //_viewModel.ModelSelectionAndFormat.SelectedText.Format[textp].CharOffset = offs.ToString() switch
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

        /// <summary>
        /// Setting the formatting on the document by the formatting information
        /// </summary>
        /// <param name="str">string, position and formatting information</param>
        private void SetFormatting(Stwf str)
        {
            //fm[0..str.length] -> fm[i] kell az i-edik karakterre rárakni
            //fm[i].style, fm[i].weight, fm[i].family, ...

            //iterate though the rtb from st to nd
            TextPointer contentStart = _view.DocPaper.Document.ContentStart;
            TextPointer st = contentStart.GetPositionAtOffset(str.SelectedText.StartPointer);
            TextPointer nd = contentStart.GetPositionAtOffset(str.SelectedText.EndPointer);

            TextPointer ptr = st;

            int prevOffsetToPos = contentStart.GetOffsetToPosition(ptr)-1;

            int fi = 0;
            while (ptr != nd && ptr != null && ptr.GetNextInsertionPosition(LogicalDirection.Forward) != null && fi < str.SelectedText.SelectedString.Length)
            {
                TextRange tr = new TextRange(ptr, ptr.GetNextInsertionPosition(LogicalDirection.Forward));
                if (tr.Text != "")
                {
                    
                    if (prevOffsetToPos != 0 && prevOffsetToPos == contentStart.GetOffsetToPosition(ptr))
                    {
                        ptr = ptr.GetNextInsertionPosition(LogicalDirection.Forward);
                    }
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
                    fi++;
                }
                if (prevOffsetToPos == contentStart.GetOffsetToPosition(ptr))
                {
                    prevOffsetToPos += 1;
                }
                else
                {
                    prevOffsetToPos = contentStart.GetOffsetToPosition(ptr);
                }
                ptr = ptr.GetNextInsertionPosition(LogicalDirection.Forward);

                
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
            _view.DocPaper.Document.Blocks.Clear();
            Paragraph newPr = new Paragraph();
            _view.DocPaper.Document.Blocks.Add(newPr);
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
