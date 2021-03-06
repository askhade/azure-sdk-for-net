﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.Management.Compute;
using Microsoft.Azure.Management.Compute.Models;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.Storage.Models;
using Microsoft.Azure.Test.HttpRecorder;
using Microsoft.Rest.ClientRuntime.Azure.TestFramework;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Linq;
using Xunit;

namespace Compute.Tests
{
    public class LogAnalyticsTests : VMTestBase
    {
        [Fact(Skip = "ReRecord due to CR change")]
        public void TestExportingThrottlingLogs()
        {
            using (MockContext context = MockContext.Start(this.GetType().FullName))
            {
                EnsureClientsInitialized(context);

                string rg1Name = ComputeManagementTestUtilities.GenerateName(TestPrefix);
                
                string storageAccountName = ComputeManagementTestUtilities.GenerateName(TestPrefix);

                try
                {
                    string sasUri = GetBlobContainerSasUri(rg1Name, storageAccountName);

                    RequestRateByIntervalInput requestRateByIntervalInput = new RequestRateByIntervalInput()
                    {
                        BlobContainerSasUri = sasUri,
                        FromTime = DateTime.UtcNow.AddDays(-10),
                        ToTime = DateTime.UtcNow.AddDays(-8),
                        IntervalLength = IntervalInMins.FiveMins,
                    };

                    LogAnalyticsOperationResult result = m_CrpClient.LogAnalytics.ExportRequestRateByInterval(requestRateByIntervalInput, "westcentralus");

                    Assert.Equal("Succeeded", result.Status);
#if NET46
                    Assert.True(result.Properties.Output.EndsWith(".csv"));
#else
                    Assert.EndsWith(".csv", result.Properties.Output);
#endif

                    ThrottledRequestsInput throttledRequestsInput = new ThrottledRequestsInput()
                    {
                        BlobContainerSasUri = sasUri,
                        FromTime = DateTime.UtcNow.AddDays(-10),
                        ToTime = DateTime.UtcNow.AddDays(-8),
                        GroupByOperationName = true,
                    };

                    result = m_CrpClient.LogAnalytics.ExportThrottledRequests(throttledRequestsInput, "westcentralus");

                    Assert.Equal("Succeeded", result.Status);
#if NET46
                    Assert.True(result.Properties.Output.EndsWith(".csv"));
#else
                    Assert.EndsWith(".csv", result.Properties.Output);
#endif
                }
                finally
                {
                    m_ResourcesClient.ResourceGroups.Delete(rg1Name);
                }
            }
        }

        private string GetBlobContainerSasUri(string rg1Name, string storageAccountName)
        {
            string sasUri = "foobar";

            if (HttpMockServer.Mode == HttpRecorderMode.Record)
            {
                StorageAccount storageAccountOutput = CreateStorageAccount(rg1Name, storageAccountName);
                var accountKeyResult = m_SrpClient.StorageAccounts.ListKeysWithHttpMessagesAsync(rg1Name, storageAccountName).Result;
                CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentials(storageAccountName, accountKeyResult.Body.Key1), useHttps: true);

                var blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("sascontainer");
#if NET46
                container.CreateIfNotExists();
#else
                container.CreateIfNotExistsAsync();
#endif
                sasUri = GetContainerSasUri(container);
            }

            return sasUri;
        }

        private string GetContainerSasUri(CloudBlobContainer container)
        {
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5);
            sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24);
            sasConstraints.Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write;

            //Generate the shared access signature on the blob, setting the constraints directly on the signature.
            string sasContainerToken = container.GetSharedAccessSignature(sasConstraints);

            //Return the URI string for the container, including the SAS token.
            return container.Uri + sasContainerToken;
        }
    }
}