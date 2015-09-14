IplasEnterFunction = function () {
    var ExportFrm = Ext.create('Ext.form.Panel', {
        id: 'ExportFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/WareHouse/IplasUploadExcelEnter',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'filefield',
                margin: '10 0 10 0',
                name: 'IplasImportExcelFile',
                id: 'IplasImportExcelFile',
                fieldLabel: '料位轉移匯入',
                msgTarget: 'side',
                labelWidth: 80,
                width: 360,
                buttonText: '瀏覽..',
                submitValue: true,
                allowBlank: false,
                fileUpload: true,
                validator:
                    function (value) {
                        var type = value.split('.');
                        if (type[type.length - 1] == 'xls' || type[type.length - 1] == 'xlsx') {
                            return true;
                        }
                        else {
                            return '上傳文件類型不正確！';
                        }
                    }
            },

             {
                 html: '<font color=red>注意事項：<br/>1.匯入時第一列為[商品細項編號],第二列為[原料位],第三列為[新料位]<br/>2.匯入數據時要逐條檢查,會比較耗時,請耐心等待<br/>3.當匯入數據量較多時,建議分割匯入.<br/></font>' + "<br/><a href='#' onclick='updownDome()'>點擊下載模板</a>"
                 // bodyStyle: "padding:5px;background:#7DC64C",
                 ,
                 height: 40,
                 border: 0

             }
        ],
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
                            IplasImportExcelFile: Ext.htmlEncode(Ext.getCmp('IplasImportExcelFile').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                var total = result.total;//匯入數據條數
                                if (parseInt(total) > 0) {//如果匯入成數量大於0
                                    if (parseInt(result.error) > 0) {//如果匯入失敗數量大於0
                                        //商品不存在或者條碼重複
                                        Ext.Msg.alert("提示信息", "匯入成功數據共" + total + "條<br/>" + "異常數據共" + result.error + "條<br/>" + "其中包括<br/>" + "商品原料位不可用" + result.locid_lock + "條<br/>" + "商品細項編號不存在" + result.item_idcount + "條<br/>" + "內容不符合格式" + result.comtentcount + "條<br/>" + "商品新料位不可用" + result.item_id_have_locid + "條<br/>" + "<a href='#' onclick='updownError()'>點擊下載未能匯入數據</a>");
                                    }
                                    else {
                                        Ext.Msg.alert("提示信息", "匯入成功數據共" + total + "條<br/>");
                                    }
                                }
                                else {
                                    if (parseInt(result.error) > 0) {//如果匯入失敗數量大於0
                                        Ext.Msg.alert("提示信息", "異常數據共" + result.error + "條<br/>" + "其中包括<br/>" + "商品原料位不可用" + result.locid_lock + "條<br/>" + "商品細項編號不存在" + result.item_idcount + "條<br/>" + "內容不符合格式" + result.comtentcount + "條<br/>" + "商品新料位不可用" + result.item_id_have_locid + "條<br/>" + "<a href='#' onclick='updownError()'>點擊下載未能匯入數據</a>");

                                    }
                                    else {
                                        Ext.Msg.alert("提示信息", "目前沒有新的數據可以匯入");
                                    }
                                }
                            }
                            else {
                                Ext.Msg.alert("提示", FAILURE);
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

    var ExportWinF = Ext.create('Ext.window.Window', {
        iconCls: 'icon-user-edit',
        id: 'ExportWinF',
        width: 400,
        height: 250,
        y: 100,
        layout: 'fit',
        items: [ExportFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: '是否關閉',
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm("提示", "確認關閉？", function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('ExportWinF').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }
        ]
    });
    ExportWinF.show();
}
function updownDome() {
    window.open("/WareHouse/IplasEnterTemplate");
}
function updownError() {
    window.open("/WareHouse/IplasEntermessage");
}