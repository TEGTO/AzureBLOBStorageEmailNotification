## [WebSite](https://blobstorageemailnotification.azurewebsites.net/)

### Test task for the Net Trainee Camp interview, second stage. 

You need to create azure blob storage, connect it to the front-end with blazor (at least I used it), which has a form with email and drag&drop field. 
Then make azure trigger function, which when a new object is added to the storage notifies the user by email and sends him a link to download the file.

 <sup>* Everything was done by me </sup>
### Task:
```
1. Register an account on Azure portal (https://portal.azure.com/).
2. Create Azure Blob storage account.
3. Create Azure Web App for .Net
4. Create ASP.NET WEB application and deploy it to Azure Web App.
5. UI for web app can be one of: Blazor, Angular or React.
6. On the web app on start must be only one page with Form where user can upload a
file (must be added validation for only .docx files) and add the user email (validation for
email).
7. Web Application is putting that file to the BLOB storage.
8. Create Azure Function with BLOB storage trigger from already created BLOB and
when file is added to blob the email is sent to the user with notification the file is
successfully uploaded. The URL to the file must be secured with SAS token on the
BLOD storage. SAS token must be valid only for 1 hour.
9. Unit tests for backend logic of web app and azure function must be added.
```
__________________________________
![image](https://github.com/TEGTO/AzureBLOBStorageEmailNotification/assets/90476119/bfded4f3-a2f0-4a27-8dc3-99b558264507)

![image](https://github.com/TEGTO/AzureBLOBStorageEmailNotification/assets/90476119/de848592-b5bb-4c9e-b51f-37d13835fee3)

