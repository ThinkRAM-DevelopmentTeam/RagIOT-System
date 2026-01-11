using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Collections.Generic;

var connectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10010/devstoreaccount1;";
var containerName = "manuals";

// Get SampleData path - relative to the root of the project, not the binary location
var sampleDataPath = args.Length > 0 ? args[0] : Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "SampleData");

// Normalize the path
sampleDataPath = Path.GetFullPath(sampleDataPath);

try
{
    Console.WriteLine("Connecting to Azurite Blob Storage...");
    var serviceClient = new BlobServiceClient(connectionString);
    var containerClient = serviceClient.GetBlobContainerClient(containerName);
    containerClient.CreateIfNotExists();
    
    var files = Directory.GetFiles(sampleDataPath, "*.txt");
    
    if (files.Length == 0)
    {
        Console.WriteLine("No .txt files found in SampleData folder");
        return 0;
    }
    
    Console.WriteLine($"Uploading {files.Length} files to '{containerName}' container...");
    
    foreach (var filePath in files)
    {
        var fileName = Path.GetFileName(filePath);
        Console.WriteLine($"  Uploading: {fileName}");
        
        var blobClient = containerClient.GetBlobClient(fileName);
        using (var fileStream = File.OpenRead(filePath))
        {
            blobClient.Upload(fileStream, overwrite: true);
        }
        Console.WriteLine($"     ✓ Uploaded successfully");
    }
    
    Console.WriteLine($"\n✅ All {files.Length} files uploaded to Azurite!");
    Console.WriteLine($"Container: {containerName}");
    Console.WriteLine($"Endpoint: http://127.0.0.1:10010");
    return 0;
}
catch (Exception ex)
{
    Console.WriteLine($"ERROR: {ex.Message}");
    Console.WriteLine($"\nFallback: You can manually upload files using:");
    Console.WriteLine($"  1. Azure Storage Explorer (connect to http://127.0.0.1:10010)");
    Console.WriteLine($"  2. Create container 'manuals' and upload .txt files from {sampleDataPath}");
    return 1;
}
