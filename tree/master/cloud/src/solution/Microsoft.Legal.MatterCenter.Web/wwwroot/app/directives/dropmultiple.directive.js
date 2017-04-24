var matterMain = angular.module("matterMain");

matterMain.directive('droppablemultipleitems', function () {
    return{
        scope :{
            drop:'&',
            folder: '='
        },
        link:function(scope, element){
            var el = element[0];


            el.addEventListener('dragover', function(e){
                e.dataTransfer.dropEffect = 'move'
                if (e.preventDefault) e.preventDefault();
            }, false);


            el.addEventListener('dragenter', function (e) {
                    this.classList.add('folderDragOver');
                    return false;
            }, false);

            el.addEventListener('dragleave', function(e){
                this.classList.remove('folderDragOver');
                return false;
            }, false);

            el.addEventListener('drop', function(e){
                if(e.stopPropogation) e.stopPropogation();
                this.classList.remove('folderDragOver');
                var folderId = this.id
                var sourceItems = [];
                e.stopPropagation();
                e.stopImmediatePropagation();
                e.preventDefault();
               
                //Check if the file has been dropped from the users desktop
                if (e.dataTransfer && e.dataTransfer.files && e.dataTransfer.files.length != 0) {
                    if (e.preventDefault) { e.preventDefault(); }
                    //Need to handler files that has been dragged from the user desktop
                    var sourceFiles = e.dataTransfer.files;

                    var isOverwrite = false;
                    var vmControlObj = scope.$parent.$parent.$parent.$parent.$parent.$parent.$parent.$parent.$parent.$parent.$parent.grid.appScope.$parent.vm;
                  vmControlObj.clientRelativeUrl = scope.folder.url;
                  vmControlObj.files = sourceFiles;
                  vmControlObj.handleDesktopDrop(scope.folder.url, sourceFiles, isOverwrite);
                }
                else {
                    //Get the current item that is getting dragged
                   
                    var item = document.getElementById(e.dataTransfer.getData('Text'));
                   
                        var isEmail = item.dataset.isemail;
                        var title = "";
                        var mailId = '';
                        if (isEmail == 'true') {
                            mailId = item.dataset.attachmentid
                        }
                        else {
                            mailId = item.dataset.parentmailid
                        }
                        title = item.title;
                        if (item.title == "") {
                            title = item.dataset.name;
                        }
                        var sourceItem = {
                            //Construct the JSON object for the dragged item                    
                            title: title,
                            attachmentId: item.dataset.attachmentid,
                            contentType: item.dataset.contenttype,
                            size: item.dataset.size,
                            isEmail: item.dataset.isemail,
                            attachmentType: item.dataset.attachmenttype,
                            i: item.id,
                            name: item.dataset.name,
                            mailId: mailId,
                            subject: item.dataset.subject
                        }
                        //scope.folder contains the target folder information that the current item is getting dropped to
                        //call the parent method called handleDrop which will call the web api method to upload the attachment
                        if (scope.$parent.vm) {
                            scope.$parent.vm.handleOutlookDrop(scope.folder, sourceItem);
                        } else {
                           
                           //  scope.$parent.$parent.$parent.$parent.$parent.$parent.$parent.$parent.$parent.grid.appScope.$parent.vm.handleOutlookDrop(scope.folder, sourceItem);
                           scope.$parent.$parent.$parent.$parent.$parent.$parent.$parent.$parent.$parent.grid.appScope.$parent.vm.handleMultipleEmails(scope.folder, sourceItem);
                      
                        }
                   
                }
                
               return false;
            }, false);
        }
    };
});