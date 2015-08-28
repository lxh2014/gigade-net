Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);

var htmljosn = '暫無數據';
var OrderBrandProducesListGrid = Ext.create('Ext.panel.Panel', {
    margin: '0 0 20 5',   
    id: 'frmhtml',
    html: "加載中...",
    renderTo: Ext.getBody()
});

var frm = Ext.create('Ext.form.Panel', {
    id: 'frm',
    layout: 'anchor',
    height: 30,
    border: 0,
    width: document.documentElement.clientWidth,
    items: [
        {
            xtype: 'button',
            text: '列印',
            iconCls: 'icon-search',
            margin: '0 0 10 5',
            id: 'btnQuery',
            handler: Query
        }
    ]
});
Ext.onReady(function () {
    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        items: [frm, OrderBrandProducesListGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                OrderBrandProducesListGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();

    var rowIDs = document.getElementById("rowID").value;
    Ext.Ajax.request({
        url: "/SendProduct/PickingPrintList",
        method: 'post',
        params: {
            rowIDs: rowIDs
        },
        success: function (response) {
            var result = response.responseText;
            var re = Ext.JSON.decode(result);
            if (re.success) {
                Ext.getCmp('frmhtml').update(re.msg);
            } else {
                Ext.getCmp('frmhtml').update(htmljosn);
            }
        },
        failure: function () {
            Ext.Msg.alert('加載出錯！');
        }
    });
});
function Query()
{
     bdhtml = window.document.body.innerHTML;//Ext.getCmp('frmhtml').htmljosn;//htmljosn;////
     startprint = ("<div>");
    endprint = ("</div>");
    printhtml = bdhtml.substr(bdhtml.indexOf(startprint) );//39+ 5
    printhtml = printhtml.substr(0, printhtml.indexOf(endprint));

    //window.document.body.innerHTML =printhtml;
    //window.print();

   //// javascript: print();
   // window.location.reload();

    //var pw = window.open();
    window.document.write('<html>');
    window.document.write('<head>');
    window.document.write('<title>批次檢貨明細列印</title>');
    window.document.write('</head>');
    window.document.write('<body><from>');
    window.document.write(printhtml);
    window.document.write('<from></body>');
    window.document.write('</html>');
    //pw.document.close();
    //setTimeout(function () {
    window.print();
    window.location.reload();


}