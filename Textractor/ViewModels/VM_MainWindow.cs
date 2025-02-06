using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using sbdotnet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Textractor.Models;
using Textractor.Views;

namespace Textractor.ViewModels
{
    public partial class VM_MainWindow : ViewModelBase
    {

        /////////////////////////////////////////////////////////
        #region Fields

        readonly List<FilePickerFileType> FileTypeFilters =
            [
                FilePickerFileTypes.ImageAll,
                FilePickerFileTypes.Pdf
            ];


        #endregion Fields
        /////////////////////////////////////////////////////////



        /////////////////////////////////////////////////////////
        #region Properties

        [ObservableProperty]
        string ocrResult = string.Empty;

        [ObservableProperty]
        string ocrStatus = string.Empty;

        [ObservableProperty]
        string selectedFile = string.Empty;

        [ObservableProperty]
        Bitmap? selectedImage;

        [ObservableProperty]
        int tabIndex = 0;

        #endregion Properties
        /////////////////////////////////////////////////////////



        /////////////////////////////////////////////////////////
        #region Commands

        [RelayCommand]
        public async Task OpenFile()
        {
            var toplevel = TopLevel.GetTopLevel(MainWindow.Instance);
            var files = await toplevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select the image or image document to open",
                AllowMultiple = false,
                FileTypeFilter = FileTypeFilters
            });
            if (files is null || files.Count == 0)
            {
                return;
            }
            SelectedFile = files[0].Path.AbsolutePath;
        }

        [RelayCommand(CanExecute = nameof(CanEx_Process_Full))]
        private void Process_Full()
        {
            OcrResult = Tocr.Process(SelectedFile);
        }

        private bool CanEx_Process_Full()
        {
            return !SelectedFile.IsNull();
        }

        [RelayCommand(CanExecute = nameof(CanEx_Process_Text))]
        private void Process_Text()
        {
            OcrResult = Tocr.Process_Text(SelectedFile);
        }

        private bool CanEx_Process_Text()
        {
            return !SelectedFile.IsNull();
        }

        #endregion Commands
        /////////////////////////////////////////////////////////


        public VM_MainWindow()
        {
            PropertyChanged += VM_MainWindow_PropertyChanged;
        }

        private void VM_MainWindow_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName is not null)
            {
                if (e.PropertyName.Equals(nameof(SelectedFile)))
                {
                    Process_FullCommand.NotifyCanExecuteChanged();
                    Process_TextCommand.NotifyCanExecuteChanged();

                    if (!SelectedFile.IsNull())
                    {
                        LoadImage();
                    }
                }

                else if (e.PropertyName.Equals(nameof(OcrResult)))
                {
                    TabIndex = 0;
                }

                else if (e.PropertyName.Equals(nameof(SelectedImage)))
                {
                    TabIndex = 1;
                }
            }
        }

        private void LoadImage()
        {
            try
            {
                using var stream = new FileStream(SelectedFile, FileMode.Open);
                SelectedImage = Bitmap.DecodeToWidth(stream, 800);
            }
            catch (Exception ex)
            {
                sbdotnet.Logger.Error(ex);
            }
        }
    }
}
