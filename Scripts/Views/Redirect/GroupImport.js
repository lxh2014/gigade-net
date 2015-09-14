Ext.onReady(function () {
    //document.getElementById('hidden_group_id').value 獲取到對應的值group_id 
    var group_name = "";
    Ext.Ajax.request({
        url: "/Redirect/GetGroupName",
        method: 'post',
        async: false, //true為異步，false為異步
        params: {
            group_id: document.getElementById('hidden_group_id').value
        },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                group_name = result.data;
            }
        }
    });
    var exportTab = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        width: 600,
      //url: '/Redirect/RedirectInto',//批次商品類別匯入
        url:'/Redirect/RedirectUploadExcel',//excel
        defaults: {
            labelWidth: 150,
            width: 400,
            margin: '20 10 0 20'
        },
        border: false,
        plain: true,
        id: 'InvoiceCheck',
        items: [
            {
                xtype: 'displayfield',
                id: 't_group_id',
                name: 't_group_id',
                fieldLabel: '群組編號',
                value: document.getElementById('hidden_group_id').value
            },
            {
                xtype: 'displayfield',
                id: 't_group_name',
                name: 't_group_name',
                fieldLabel: '群組名稱',
                value:group_name
            },
        {
            xtype: 'filefield',
            name: 'ImportExcelFile',
            id: 'ImportExcelFile',
            fieldLabel: '匯入Excel檔案',
            msgTarget: 'side',
            buttonText: '瀏覽..',
            submitValue: true,
            allowBlank: false,
            fileUpload: true,
            validator:
                function (value) {
                    var type = value.split('.');
                    if (type[type.length - 1] == 'csv') {
                        return true;
                    } else if (type[type.length - 1] == 'xls')
                    {
                        return true;
                    }
                    else {
                        return '上傳文件類型不正確！';
                    }
                }
        }
        ],
        buttonAlign: 'right',
        buttons: [{
            text: '確定匯入',
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        waitMsg: '匯入數據中,請稍等......',
                        waitTitle: '提示',
                        timeout: 9000000,
                        params: {
                            ImportExcelFile: Ext.htmlEncode(Ext.getCmp('ImportExcelFile').getValue()),
                            group_id: document.getElementById('hidden_group_id').value,
                            group_name: group_name
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {

                                var total = result.count;//匯入數據條數
                                var errortotal = result.errorcount;
                                if (parseInt(total) > 0) {//如果匯入成數量大於0
                                    if (parseInt(errortotal) > 0) {//如果匯入失敗數量大於0
                                        
                                        Ext.Msg.alert("提示信息", "共匯入成功數據" + total + "條<br/>" + "異常數據" + errortotal + "條<br/>" + "<a href='#' onclick='updownthis()'>點擊下載未能匯入數據</a>");
                                    }
                                    else {
                                       
                                            Ext.Msg.alert("提示信息", "共匯入成功數據" + total + "條<br/>");
                                    }
                                }
                                else {
                                    if (parseInt(errortotal) > 0) {//如果匯入失敗數量大於0
                                       
                                            Ext.Msg.alert("提示信息", "異常數據" + errortotal + "條<br/>" + "<a href='#' onclick='updownthis()'>點擊下載異常數據</a>");
                                      
                                    }
                                    else {
                                      
                                            Ext.Msg.alert("提示信息", "目前沒有數據可以匯入");
                                    }
                                }
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        },
                        failure: function (form, action) {
                            Ext.Msg.alert("提示信息", "匯入失敗");
                        }
                    });
                }
            }
        }]
    });
    var hpanel = Ext.create('Ext.form.Panel', {
        bodyStyle: "padding:5px;background:#87CEEB",
        border: false,
         //html: "注意事項：<br/>1.匯出的CSV模板請用text文檔打開編輯內容,請勿將第一列欄位名稱刪除.<br/>2. 匯入CSV檔案,必須是UTF-8編碼格式,必需為逗點分隔欄位,否則匯入的檔案會是亂碼.<br/>3. CSV檔案,一列一筆資料,若資料有異常,系統主動略過不處理.<br/>4. CSV檔案,包含三個欄位，如下：<br/>欄位一：連結名稱<br/>欄位二：目的連結<br/>欄位三：連結狀態,1（正常）,2（停用）.若不為 1、2、未指定或有錯誤時,預設皆為正常<br/> 其它欄位：皆略過不處理.<br/>5.匯入CSV檔案大小預設 2MB,若超出系統限制時,請自行分割檔案後,再重新匯入.<br/>6.匯入處理速度,每秒約 100 筆資料,若名單過大,請自行切割檔案後分批匯入,以減少錯誤及影響系統效能.<br/><a href='#' onclick='updownmuban()'>點擊下載csv模板</a>"
        html: "注意事項：<br/>1.匯出的Excel模板請勿將第一列欄位名稱刪除.<br/>2. Excel檔案,一列一筆資料,若資料有異常,系統主動略過不處理.<br/>4. Excel檔案,包含三個欄位，如下：<br/>欄位一：連結名稱<br/>欄位二：目的連結<br/>欄位三：連結狀態,1（正常）,2（停用）.若不為 1、2、未指定或有錯誤時,預設皆為正常<br/> 其它欄位：皆略過不處理.<br/>5.匯入Excel檔案大小預設 2MB,若超出系統限制時,請自行分割檔案後,再重新匯入.<br/>6.匯入處理速度,每秒約 100 筆資料,若名單過大,請自行切割檔案後分批匯入,以減少錯誤及影響系統效能.<br/><a href='#' onclick='updownmuban()'>點擊下載excel模板</a>"
    });
    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        items: [exportTab, hpanel],
        autoScroll: true
    });
});
function updownmuban() {
    //window.open("/Redirect/IplasUpdownTemplate");
    window.open("/Redirect/RedirectUpdownTemplate");//excel
}
function updownthis() {
    //window.open("/Redirect/Updownmessage");
    window.open("/Redirect/UpdownmessageExcel");//excel
}
