using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using sbdotnet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
            FilePickerFileTypes.ImagePng,
            FilePickerFileTypes.ImageJpg
            ];

        static readonly int Tab_OCR = 0;
        static readonly int Tab_IMG = 1;

        #endregion Fields
        /////////////////////////////////////////////////////////



        /////////////////////////////////////////////////////////
        #region Properties

        [ObservableProperty]
        string _OcrResult = string.Empty;

        [ObservableProperty]
        string _OcrStatus = string.Empty;

        [ObservableProperty]
        string _SelectedFile = string.Empty;

        [ObservableProperty]
        Bitmap? _SelectedImage;

        [ObservableProperty]
        int _TabIndex = 0;

        [ObservableProperty]
        bool _IsProcessing = false;

        #endregion Properties
        /////////////////////////////////////////////////////////



        /////////////////////////////////////////////////////////
        #region Commands

        private bool Canex_OpenFile()
        {
            return (IsProcessing == false);
        }

        [RelayCommand(CanExecute = nameof(Canex_OpenFile))]
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
            TabIndex = 0;
        }

        [RelayCommand(CanExecute = nameof(Canex_OpenFile))]
        public void TestImage01()
        {
            string appdata = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            SelectedFile = Path.Combine(appdata, "TestPic01.jpg");
            TabIndex = 0;
        }

        [RelayCommand(CanExecute = nameof(Canex_OpenFile))]
        public void TestImage02()
        {
            string appdata = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            SelectedFile = Path.Combine(appdata, "TestPic02.jpg");
            TabIndex = 0;
        }



        [RelayCommand(CanExecute = nameof(Canex_Process_Full))]
        private async Task Process_Full()
        {
            IsProcessing = true;
            OcrResult = await Task.Run(() => Tocr.Process_Full(SelectedFile));
            IsProcessing = false;
            TabIndex = 1;
        }

        private bool Canex_Process_Full()
        {
            return !SelectedFile.IsNull() && !IsProcessing;
        }

        [RelayCommand(CanExecute = nameof(Canex_Process_Text))]
        private async Task Process_Text()
        {
            IsProcessing = true;
            OcrResult = await Task.Run(() => Tocr.Process_Text(SelectedFile));
            IsProcessing = false;
            TabIndex = 1;
        }

        private bool Canex_Process_Text()
        {
            return !SelectedFile.IsNull() && !IsProcessing;
        }

        private void Update_Canex()
        {
            OpenFileCommand.NotifyCanExecuteChanged();
            Process_FullCommand.NotifyCanExecuteChanged();
            Process_TextCommand.NotifyCanExecuteChanged();
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
                    Update_Canex();

                    if (!SelectedFile.IsNull())
                    {
                        LoadImage();
                    }
                }

                else if (e.PropertyName.Equals(nameof(OcrResult)))
                {
                    TabIndex = Tab_OCR;
                }

                else if (e.PropertyName.Equals(nameof(SelectedImage)))
                {
                    TabIndex = Tab_IMG;
                }

                else if (e.PropertyName.Equals(nameof(IsProcessing)))
                {
                    Update_Canex();
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
