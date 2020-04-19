using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using JungleLibrary;

namespace Jungle.Dal
{
    /// <summary>
    /// This class handles the loading and saving
    /// </summary>
    class DataHandler
    {
        /// <summary>
        /// Load the strings from a file and give back a List<Move>
        /// </summary>
        /// <returns></returns>
        public async Task<List<Move>> LoadGame()
        {
            FileOpenPicker fileOpenPicker = new FileOpenPicker();
            fileOpenPicker.FileTypeFilter.Add( ".csv");
            StorageFile sourceFile = await fileOpenPicker.PickSingleFileAsync();
            string moveListAsString;
            List<Move> moveList=new List<Move>();
            if (sourceFile != null)
            {
                var stream = await sourceFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
                ulong size = stream.Size;
                using (var inputStream = stream.GetInputStreamAt(0))
                {
                    using (var dataReader = new Windows.Storage.Streams.DataReader(inputStream))
                    {
                        uint numBytesLoaded = await dataReader.LoadAsync((uint)size);
                        moveListAsString = dataReader.ReadString(numBytesLoaded);
                    }
                }
                stream.Dispose();

                string[] moveStringList = moveListAsString.Split(System.Environment.NewLine);
                // The first number determines how many moves in the file need to be read
                // I use this trick because I haven't found a way to empty a file
                int moveNumber = int.Parse(moveStringList[0]);      
                for (int i = 1; i <= moveNumber; i++)
                {
                    moveList.Add(Move.Parse(moveStringList[i]));
                }
            }
            return moveList;
        }

        //
        /// <summary>
        /// Accept a List<Move> and dump them into a CSV file
        /// </summary>
        /// <param name="moveList">a list of moves</param>
        public async void SaveGame(List<Move> moveList)
        {
            FileSavePicker fileSavePicker = new FileSavePicker();
            fileSavePicker.DefaultFileExtension = ".csv";
            fileSavePicker.FileTypeChoices.Add("game data", new List<string>() { ".csv" });
            StorageFile targetFile = await fileSavePicker.PickSaveFileAsync();
            if (targetFile != null)
            {
                var stream = await targetFile.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);
                using (var outputStream = stream.GetOutputStreamAt(0))
                {
                    using (var dataWriter = new Windows.Storage.Streams.DataWriter(outputStream))
                    {
                        // The first number determines how many moves should be read in the future loading
                        // I use this trick because I haven't found a way to empty a file
                        dataWriter.WriteString(moveList.Count.ToString()+ System.Environment.NewLine);
                        foreach (Move move in moveList)
                            dataWriter.WriteString(move.ToString() + System.Environment.NewLine);
                        await dataWriter.StoreAsync();
                        await outputStream.FlushAsync();
                    }
                }
                stream.Dispose();
            }
        }
    }
}
