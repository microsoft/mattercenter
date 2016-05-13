var matterMain = angular.module("matterMain");

matterMain.directive('droppable', function () {
    return{
        scope :{
            drop:'&',
            folder: '='
        },
        link:function(scope, element){
            var el = element[0];


            el.addEventListener('dragover', function(e){
                e.dataTransfer.dropEffect = 'move'
                if(e.preventDefault) e.preventDefault();
                this.classList.add('over');
                return false;
            }, false);


            el.addEventListener('dragenter', function(e){
                this.classList.add('over');
                return false;
            }, false);

            el.addEventListener('dragleave', function(e){
                this.classList.remove('over');
                return false;
            }, false);

            el.addEventListener('drop', function(e){
                if(e.stopPropogation) e.stopPropogation();
                this.classList.remove('over');
                var folderId = this.id
                //Get the current item that is getting dragged
                var item = document.getElementById(e.dataTransfer.getData('Text'));
                //Construct the JSON object for the dragged item
                var sourceItem = {
                    title: item.title,
                    attachmentId: item.dataset.attachmentid,
                    contentType: item.dataset.contenttype,
                    size:item.dataset.size
                }
                //scope.folder contains the target folder information that the current item is getting dropped to
                //call the parent method called handleDrop which will call the web api method to upload the attachment
                scope.$parent.vm.handleDrop(scope.folder, sourceItem)
               return false;
            }, false);
        }
    };
});