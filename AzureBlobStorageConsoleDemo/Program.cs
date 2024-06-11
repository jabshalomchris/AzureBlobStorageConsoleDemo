// See https://aka.ms/new-console-template for more information
using AzureBlobStorageConsoleDemo;

Console.WriteLine("Writing file to Azure Blob Storage...");

AzureBlobStorageHelper azureBlobStorageHelper = new AzureBlobStorageHelper("","");

// fetch the file from the your local path
string localFilePath = "path/to/your/local/file.txt";
using FileStream uploadFileStream = File.OpenRead(localFilePath);

string blobName = "file.txt"; // Name of the blob in the container

await azureBlobStorageHelper.SaveFileToStorageAsync("Files", blobName, uploadFileStream);

public static FileStream GetFileStream()
{

}