ImportFunction = function (row, store) {
    var ExportFrm = Ext.create('Ext.form.Panel', {
        id: 'ExportFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/Edm/Import',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'displayfield',
                id: 'group_id',
                name: 'group_id',
                fieldLabel: '群組編號',
                hidden: true,
            },

            {
                xtype: 'filefield',
                margin: '10 0 10 0',
                name: 'ImportCsv',
                id: 'ImportCsv',
                fieldLabel: '匯入',
                msgTarget: 'side',
                labelWidth: 70,
                width: 360,
                buttonText: '瀏覽..',
                submitValue: true,
                allowBlank: false,
                fileUpload: true,
                validator:
                    function (value) {
                        var type = value.split('.');
                        if (type[type.length - 1] == 'csv') {
                            return true;
                        }
                        else {
                            return '上傳文件類型不正確！';
                        }
                    }
            },
              {
                  xtype: 'displayfield',
                  value: "<a href='#' onclick=DownTemplate() style='font-size:large' >點擊下載模板</>",
                  margin: '0 0 0 78',
              }
              ,
             {
                 xtype: 'displayfield',
                 html: '<ol><li>CSV檔案，一行一筆資料，首行略過不處理；若資料有異常，系統主動略過不處理。</li><li><li>欄位一：電子信箱位址。</li><li>欄位二：是否訂閱電子報，0（不訂閱），1（訂閱）。若不為 0、1、未指定或有錯誤時，預設皆為訂閱。</li><li>欄位三：信箱姓名，即收信人姓名，或無指定，預設以電子信箱帳號代替。</li><li>其它欄位：皆略過不處理。</li></li><li>若此郵件群組中已有相同信箱時，系統不重覆匯入，但欄位二、欄位三資訊，會以匯入檔案資訊取代。</li></ol>'
                 // bodyStyle: "padding:5px;background:#7DC64C",
                 ,
                 height: 80,
                 border: 0

             }
             ,

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
                            ImportCsv: Ext.htmlEncode(Ext.getCmp('ImportCsv').getValue()),
                            group_id: Ext.getCmp('group_id').getValue(),
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert("提示", "匯入完成!");
                                ExportWin.close();
                                store.load();
                            }
                            else {
                                Ext.Msg.alert("提示", "匯入過程中出錯!");
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

    var ExportWin = Ext.create('Ext.window.Window', {
        iconCls: 'icon-user-edit',
        id: 'ExportWin',
        width: 400,
        height: 330,
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
                         Ext.getCmp('ExportWin').destroy();

                     }
                     else {
                         return false;
                     }
                 });
             }
         }
        ],
        listeners: {
            'show': function () {
                if (row) {
                    Ext.getCmp('group_id').setValue(row.data.group_id);
                    //editFrm.getForm().loadRecord(row);
                    //Ext.getCmp('group_id').show(true);
                    //Ext.getCmp('s_group_createdate').show(true);
                    //Ext.getCmp('s_group_updatedate').show(true);
                }
            }
        },
    });
    ExportWin.show();
}
function DownTemplate() {
    window.open("/Edm/DownTemplate");
}