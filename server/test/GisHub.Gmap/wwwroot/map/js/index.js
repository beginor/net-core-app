$(function(){
});

function slideToggleDialog(dialog){
    //获取指定对象
    function getNode(parentDiv,nodeName){
        for(var i = 0,len = parentDiv.childNodes.length; i < len; i++){
            var node = parentDiv.childNodes[i];
            if(nodeName == "content"){
                if(node.className != "dtitle" && node.nodeName == "DIV"){
                    return node;
                }
            }
            else if(nodeName == "img"){
                if(node.className == "dtitle"){
                    var img = node.getElementsByTagName("img")[0];
                    return img;
                }
            }
            else if(nodeName == "iframe"){
                if(node.nodeName == "IFRAME" && node.className == "bgiframe bgiframe-bottom"){
                    return node;
                }
            }
        }
    }
    //获取弹出div对象
    var dialog = document.getElementById(dialog);
    //获取图片对象
    var img = getNode(dialog,"img");
    //处理
    if(dialog.style.height == "auto"){
        dialog.style.height = "18px";
        img.src = "./img/openD.png";
    }else{
        dialog.style.height = "auto";
        img.src = "./img/closeD.png";
    }
}