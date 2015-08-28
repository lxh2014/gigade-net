Ext.onReady(function () {
    var exportTab = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        width: 600,
        url: '/Member/ExportCsv',
        defaults: {
            labelWidth: 100,
            width: 400,
            margin: '20 10 0 20'
        },
        border: false,
        plain: true,
        id: 'InvoiceCheck',
        items: [
        {
            xtype: 'filefield',
            name: 'ImportCsvFile',
            id: 'ImportCsvFile',
            fieldLabel: '匯入csv檔案',
            msgTarget: 'side',
            buttonText: '瀏覽..',
            submitValue: true,
            allowBlank: false,
            fileUpload: true

        }
         ],
        buttonAlign: 'right',
        buttons: [{
            text: '新增',
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {

                            ImportCsvFile: Ext.htmlEncode(Ext.getCmp('ImportCsvFile').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                files = result.msg;
                                window.location = "../../ImportUserIOExcel/"+files;
                                //                                Ext.Msg.alert(INFORMATION, '<a href="../../ImportUserIOExcel/' + files + '">報表下載</a>');
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        },
                        failure: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg, alert(result.msg);
                        }
                    });
                }
            }
        }]
    });
    var hpanel = Ext.create('Ext.form.Panel', {
        bodyStyle: "padding:5px",
        border: false,
        html: "注意事項：<br/><br/>1.匯入CSV檔案，必需為逗點分隔欄位。<br/>2.檔案格式請使用UTF-8編碼，否則無法匯入。<br/>3.CSV檔案，第一行資訊，將主動略過不處理。<br/>4.CSV檔案，一列一筆資料，若資料有異常，系統將中斷。<br/>5.CSV檔案，包含一個欄位，如下： 欄位一：會員ID。其它欄位：皆略過不處理。<br/>6.匯入CSV檔案大小預設 2MB，若超出系統限制時，請自行分割檔案後，再重新匯入。<br/>7.匯入處理速度，每秒約 100 筆資料，若名單過大，請自行切割檔案後分批匯入，以減少錯誤及影響系統效能。"
    });
    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        items: [exportTab, hpanel],
        autoScroll: true
    });
});