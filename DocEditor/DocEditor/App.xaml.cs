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
using DocEditor.Model;
using DocEditor.ViewModel;


namespace DocEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        struct SelectionOffsets { internal int Start; internal int End; }
        private DocEditorModel _model;
        private MainViewModel _viewModel;
        private MainWindow _view;
        private HomeWindow _home;
        private PdfViewer _dictHelp;

        private FormatTextViewModel _ftViewModel;
        PageSettingsViewModel _pageViewModel;


        public App()
        {
            Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            _model = new DocEditorModel();

            _viewModel = new MainViewModel(_model);
            _viewModel.ExitApp += new EventHandler(ViewModel_Exit);
            _viewModel.MinApp += new EventHandler(ViewModel_Min);
            _viewModel.MaxApp += new EventHandler(ViewModel_Max);

            _viewModel.OpenHome += new EventHandler(ViewModel_OpenHomeWindow);
            _viewModel.CloseHome += new EventHandler(Home_Close);
            _viewModel.MinHome += new EventHandler(Home_Min);
            _viewModel.MaxHome += new EventHandler(Home_Max);

            _viewModel.OpenFormatText += new EventHandler(ViewModel_FormatText);
            _viewModel.OpenEditPage += new EventHandler(ViewModel_EditPage);
            _viewModel.OpenInsert += new EventHandler(ViewModel_Insert);
            _viewModel.OpenDict += new EventHandler(ViewModel_Dict);

            _viewModel.OpenDictHelp += new EventHandler(ViewModel_OpenDictHelp);

            _ftViewModel = new FormatTextViewModel(_model);
            _ftViewModel.OpenColorPicker += new EventHandler(openColorDialog);
            _ftViewModel.AddMoreFormatting += new EventHandler(openMoreFormatting);
            _ftViewModel.AddMoreSpacing += new EventHandler(openMoreSpacing);
            
            _ftViewModel.FontFamilyChanged += new EventHandler<SelectionChangedEventArgs>(Change_FontFamily);
            _ftViewModel.FontSizeChanged += new EventHandler<TextChangedEventArgs>(Change_FontSize);
            _ftViewModel.RTBFocus += new EventHandler(RTB_GetFocusBack);

            _pageViewModel = new PageSettingsViewModel(_model);
            _pageViewModel.MorePageSettings += new EventHandler(openMorePageSettings);


            _view = new MainWindow
            {
                DataContext = _viewModel
            };
            _view.Show();
        }

        #region ViewModel event handlers
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
            _home.Close();
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

        /// <summary>
        /// Main Window event handlers
        /// </summary>
        private void ViewModel_Exit(object sender, EventArgs e)
        {
            Shutdown();
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
                //_view.Activate();
            }
            else
            {
                _view.WindowState = WindowState.Normal;
            }
        }

        private void ViewModel_FormatText(object sender, EventArgs e)
        {
            //FormatTextViewModel _ftViewModel = new FormatTextViewModel(_model);
            _viewModel.CurrentView = _ftViewModel;
            //SubscribeToEvents(_ftViewModel);
        }

        private void ViewModel_EditPage(object sender, EventArgs e)
        {
            //PageSettingsViewModel _pageViewModel = new PageSettingsViewModel(_model);
            _viewModel.CurrentView = _pageViewModel;
        }

        private void ViewModel_Insert(object sender, EventArgs e)
        {
            InsertViewModel _insViewModel = new InsertViewModel(_model);
            _viewModel.CurrentView = _insViewModel;
        }

        private void ViewModel_Dict(object sender, EventArgs e)
        {
            DictionaryViewModel _dictViewModel = new DictionaryViewModel(_model);
            _viewModel.CurrentView = _dictViewModel;
        }

        private void SubscribeToEvents(ViewModelBase viewModel)
        {
            if (viewModel is FormatTextViewModel)
            {
                FormatTextViewModel ftViewModel = viewModel as FormatTextViewModel;
            }
        }

        private void openColorDialog(object sender, EventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //elmenteni egy változóba, amit rárakok a kijelölt szövegre
                _ftViewModel.STextColor = colorDialog.Color.Name.ToLower();
            }
        }

        private void openMoreFormatting(object sender, EventArgs e)
        {
            FontListWindow _window = new FontListWindow
            {
                DataContext = _ftViewModel
            };
            _window.ShowDialog();
        }

        private void openMoreSpacing(object sender, EventArgs e)
        {
            Spacing _window = new Spacing
            {
                DataContext = _ftViewModel
            };
            _window.ShowDialog();
        }

        private void ViewModel_OpenDictHelp(object sender, EventArgs e)
        {
            _dictHelp = new PdfViewer
            {
                DataContext = _viewModel
            };
            _dictHelp.Title = "Dictionary help";
            _dictHelp.ShowDialog();
        }

        private void ViewModel_Copy(object sender, ExecutedRoutedEventArgs e)
        {
            using (var stream = new MemoryStream())
            {
                _view.DocPaper.Selection.Save(stream, DataFormats.Rtf);
                stream.Position = 0;
                //gembox document
                //DocumentModel.Load(stream, LoadOptions.RtfDefault).Content.SaveToClipboard();
            }
        }

        private void ViewModel_Paste(object sender, ExecutedRoutedEventArgs e)
        {
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

        private void ViewModel_Cut(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel_Copy(sender, e);

            _view.DocPaper.Selection.Text = string.Empty;

        }

        private void Change_FontFamily(object sender, SelectionChangedEventArgs e)
        {
            //fontsizecmb leküldi a modellbe, és az alapján állítjuk be itt 
            //FontFamilyCmb.SelectedItem
            //if (_view.FontFamilyCmb.SelectedItem != null)
            if(_ftViewModel.SFontFamily != null)
            {
                _view.DocPaper.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, _model.FormatText.Family);
            }
        }

        private void Change_FontSize(object sender, TextChangedEventArgs e)
        {
            //fontsizecmb leküldi a modellbe, és az alapján állítjuk be itt 
            //FontSizeCmb.Text
            _view.DocPaper.Selection.ApplyPropertyValue(Inline.FontSizeProperty, _model.FormatText.Size);
        }

        private void RTB_GetFocusBack(object sender, EventArgs e)
        {
            Keyboard.Focus(_view.DocPaper);
            _view.DocPaper.Selection.Select(_view.DocPaper.Selection.Start, _view.DocPaper.Selection.End);
        }


        private void openMorePageSettings(object sender, EventArgs e)
        {
            PageSettingsWindow _window = new PageSettingsWindow
            {
                DataContext = _pageViewModel
            };
            _window.ShowDialog();
        }

        /*private void Change_FontFamily(object sender, EventArgs e)
        {
            _model.FormatText.Family = _ftViewModel.SFontFamily;
        }

        private void Change_FontSize(object sender, EventArgs e)
        {
            _model.FormatText.Size = _ftViewModel.SFontSize;
        }*/

        private void setSelection(object sender, EventArgs e)
        {
            SelectionOffsets selectionOffsets;
            TextPointer contentStart = _view.DocPaper.Document.ContentStart;
            //_model.select = new Selection(0);

            selectionOffsets.Start = contentStart.GetOffsetToPosition(_view.DocPaper.Selection.Start);
            selectionOffsets.End = contentStart.GetOffsetToPosition(_view.DocPaper.Selection.End);

            if (_view.DocPaper.Selection.Text != null && (_model.select.StartPointer == selectionOffsets.Start || _model.select.EndPointer == selectionOffsets.End))
            {
                _model.select.AddToSelected(_view.DocPaper.Selection.Text, selectionOffsets.Start, selectionOffsets.End);
            }
            else
            {
                //delete selection
                _model.select.DeleteSelection();
            }

            //ha már nincs kijelölve, akkor tötölni a modellből a korábbi kijelölést
            //onnan tudjuk, hogy adott dolog kijelölése történik, hogy vagy a start vagy az end változatlan
        }


        #endregion

    }
}
