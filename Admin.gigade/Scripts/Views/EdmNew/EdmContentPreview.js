PreviewFun = function (template_id, checked) {

    var htmlsrc;
    var subscribe_url = document.getElementById('subscribe_url').value;
    var editor = $(this).data("kendoEditor");

    try{
        if(document.getElementById('editor').value){
            htmlsrc= Ext.htmlDecode(document.getElementById('editor').value) +"<br/>"+ Ext.htmlDecode(document.getElementById('editor2').value) 
        }
    }
    catch(e){
        htmlsrc = Ext.htmlDecode(document.getElementById('editor3').value)  //$('.editor3]').data("kendoEditor");// Ext.getCmp('kendoEditor').getValue();
    }
    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "生成預覽中..." });
    myMask.show();
    Ext.Ajax.request({
        url: '/EdmNew/GetContentUrl',
        params: {
            template_id: template_id,
            template_data: htmlsrc,
        },
        success: function (data) {
            myMask.hide();
            var result = data.responseText;
            var html;
            if (checked == true) {
                html = result + subscribe_url;
            }
            else {
                html = result;
            }
            var A = 1000;
            var B = 700;
            var C = (document.body.clientWidth - A) / 2;
            var D = window.open('', null, 'toolbar=yes,location=no,status=yes,menubar=yes,scrollbars=yes,resizable=yes,width=' + A + ',height=' + B + ',left=' + C);
            var E = "<html><head><title>預覽</title></head><style>body{line-height:200%;padding:50px;}</style><body><div >" + html + "</div></body></html>";
            D.document.write(E);
            D.document.close();
        }
    });
}