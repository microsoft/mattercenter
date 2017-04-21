
var matterMain = angular.module("matterMain");
matterMain.directive('draggablemultipleemails', function(){
    return function(scope, element){
        var el = element[0];
        el.draggable = true;
        el.setAttribute('draggable', 'true');
        el.setAttribute('aria-grabbed', 'false');
        el.setAttribute('tabindex', '0');

       
        function addSelection(item) {
            item.setAttribute('aria-grabbed', 'true');
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

            scope.$parent.$parent.vm.emailSelections.items.push(sourceItem);
            console.log(scope.$parent.$parent.vm.emailSelections.items);
        }

        function removeSelection(item) {
            //reset this item's grabbed state
            item.setAttribute('aria-grabbed', 'false');

            //then find and remove this item from the existing items array
            for (var len =  scope.$parent.$parent.vm.emailSelections.items.length, i = 0; i < len; i++) {
                if ( scope.$parent.$parent.vm.emailSelections.items[i] == item) {
                     scope.$parent.$parent.vm.emailSelections.items.splice(i, 1);
                    break;
                }
            }
        }

        function clearSelections() {
            //if we have any selected items
            if ( scope.$parent.$parent.vm.emailSelections.items.length) {
                //reset the owner reference
                 scope.$parent.$parent.vm.emailSelections.owner = null;
                
                //then reset the items array        
                 scope.$parent.$parent.vm.emailSelections.items = [];
            }
        }


        //shorctut function for testing whether a selection modifier is pressed
        function hasModifier(e) {
            return (e.ctrlKey || e.metaKey || e.shiftKey);
        }


        //mousedown event to implement single selection
        el.addEventListener('mousedown', function (e) {
            //if the element is a draggable item
            if (e.target.getAttribute('draggable')) {
               
                //if the multiple selection modifier is not pressed 
                //and the item's grabbed state is currently false
                if(
                    !hasModifier(e)
                    &&
                    e.target.getAttribute('aria-grabbed') == 'false'
                ) {
                    //clear all existing selections
                    clearSelections();

                    //then add this new selection
                    addSelection(e.target);
                }
            }

                //else [if the element is anything else]
                //and the selection modifier is not pressed 
            else if (!hasModifier(e)) {
              

                //clear all existing selections
                clearSelections();
            }

                //else [if the element is anything else and the modifier is pressed]
            else {
                //clear dropeffect from the target containers
               // clearDropeffects();
            }

        }, false);


        //mouseup event to implement multiple selection
        el.addEventListener('mouseup', function (e) {
            //if the element is a draggable item 
            //and the multipler selection modifier is pressed
            if (e.target.getAttribute('draggable') && hasModifier(e)) {
                //if the item's grabbed state is currently true
                if (e.target.getAttribute('aria-grabbed') == 'true') {
                    //unselect this item
                    removeSelection(e.target);

                    //if that was the only selected item
                    //then reset the owner container reference
                    if (! scope.$parent.$parent.vm.emailSelections.items.length) {
                         scope.$parent.$parent.vm.emailSelections.owner = null;
                    }
                }

                    //else [if the item's grabbed state is false]
                else {
                    //add this additional selection
                    addSelection(e.target);
                }
            }

        }, false);


        //dragstart event to initiate mouse dragging
        el.addEventListener('dragstart', function (e) {            
            if(
                hasModifier(e)
                &&
                e.target.getAttribute('aria-grabbed') == 'false'
            ) {
                //add this additional selection
                addSelection(e.target);
            }

            //we don't need the transfer data, but we have to define something
            //otherwise the drop action won't work at all in firefox
            //most browsers support the proper mime-type syntax, eg. "text/plain"
            //but we have to use this incorrect syntax for the benefit of IE10+
            e.dataTransfer.setData('text', '');

        }, false);

     

      

        //dragend event to implement items being validly dropped into targets,
        //or invalidly dropped elsewhere, and to clean-up the interface either way
        el.addEventListener('dragend', function (e) {
            //if we have a valid drop target reference
            //(which implies that we have some selected items)
            if ( scope.$parent.$parent.vm.emailSelections.droptarget) {
                //append the selected items to the end of the target container
                for (var len =  scope.$parent.$parent.vm.emailSelections.items.length, i = 0; i < len; i++) {
                     scope.$parent.$parent.vm.emailSelections.droptarget.appendChild( scope.$parent.$parent.vm.emailSelections.items[i]);
                }

                //prevent default to allow the action            
                e.preventDefault();
            }

            //if we have any selected items
            if ( scope.$parent.$parent.vm.emailSelections.items.length) {
              

                //if we have a valid drop target reference
                if ( scope.$parent.$parent.vm.emailSelections.droptarget) {
                    //reset the selections array
                    clearSelections();

                    //reset the target's dragover class
                     scope.$parent.$parent.vm.emailSelections.droptarget.className =
                         scope.$parent.$parent.vm.emailSelections.droptarget.className.replace(/ dragover/g, '');

                    //reset the target reference
                     scope.$parent.$parent.vm.emailSelections.droptarget = null;
                }
            }

        }, false);






    }
});