using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.IO;
using CatalogAPI.Models;
using Microsoft.WindowsAzure.Storage.Table;

namespace CatalogAPI.Helpers
{
    public class StorageAccountHelper
    {
        private string storageConnectionString;
        private string tableConnectionString;

        private CloudStorageAccount storageAccount;
        private CloudStorageAccount tableStorageAccount;

        private CloudBlobClient blobClient;
        private CloudTableClient tableClient;

        public string StorageConnectionString
        {
            get { return storageConnectionString; }
            set
            {
                this.storageConnectionString = value;
                storageAccount = CloudStorageAccount.Parse(this.storageConnectionString);
            }
        }
        //For Cosmos table api
        public string TableConnectionString
        {
            get { return tableConnectionString; }
            set
            {
                this.tableConnectionString = value;
                tableStorageAccount = CloudStorageAccount.Parse(this.tableConnectionString);
            }
        }

        public async Task<string> UploadFileToBlobAsync(string filePath,string containerName)
        {
            blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            /*To set access permission*/
            BlobContainerPermissions permissions = new BlobContainerPermissions()
            {
                PublicAccess=BlobContainerPublicAccessType.Container
            };
            await container.SetPermissionsAsync(permissions);
            /*end*/

            var fileName = Path.GetFileName(filePath);
            var blob = container.GetBlockBlobReference(fileName);
            await blob.DeleteIfExistsAsync();
            await blob.UploadFromFileAsync(filePath);

            return blob.Uri.AbsoluteUri;

        }

        public async Task<CatalogEntity> SaveToTableAsyc(CatalogItem item)
        {
            CatalogEntity catalogEntity = new CatalogEntity(item.Name, item.Id)
            {
                ImageUrl=item.ImageUrl,
                ReorderLevel=item.ReorderLevel,
                Quantity=item.Quantity,
                Price=item.Price,
                ManufacturingDate=item.ManufacturingDate
            };
            //tableClient = storageAccount.CreateCloudTableClient(); //used to connect to table storage account
            tableClient = tableStorageAccount.CreateCloudTableClient(); //to connect cosmos api table
            var catalogTable = tableClient.GetTableReference("catalog");
            await catalogTable.CreateIfNotExistsAsync();
            TableOperation operation = TableOperation.InsertOrMerge(catalogEntity);
            var tableResult = await catalogTable.ExecuteAsync(operation);
            return tableResult.Result as CatalogEntity;
        }
    }
}
