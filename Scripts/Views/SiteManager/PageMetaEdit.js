editFunction = function (row) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        url: '/SiteManager/SavePageMeta',
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 80 },
        items: [
            {
                xtype: 'textfield',
                fieldLabel: 'ID',
                id: 'pm_id',
                name: 'pm_id',
                hidden: true
            },
             {
                 xtype: 'textfield',
                 fieldLabel: '名稱',
                 id: 'pm_page_name',
                 name: 'pm_page_name',
                 allowBlank: false

             },
              {
                  xtype: 'textfield',
                  fieldLabel: 'title',
                  id: 'pm_title',
                  name: 'pm_title',
                  allowBlank: false
              },
               {
                   xtype: 'textfield',
                   fieldLabel: 'url參數',
                   id: 'pm_url_para',
                   name: 'pm_url_para',
                   regex: /^[A-Za-z0-9]+$/,
                   regexText: "只能輸入英文和數字",
                   allowBlank: false
               },
                {
                    xtype: 'textfield',
                    fieldLabel: 'keywords',
                    id: 'pm_keywords',
                    name: 'pm_keywords',
                    allowBlank: false

                },
                 {
                     xtype: 'textfield',
                     fieldLabel: 'description',
                     id: 'pm_description',
                     name: 'pm_description',
                     allowBlank: false
                 }


        ],
        buttons: [{
            text: SAVE,
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            pm_id: Ext.htmlEncode(Ext.getCmp("pm_id").getValue()),
                            pm_page_name: Ext.htmlEncode(Ext.getCmp("pm_page_name").getValue()),
                            pm_title: Ext.htmlEncode(Ext.getCmp("pm_title").getValue()),
                            pm_url_para: Ext.htmlEncode(Ext.getCmp("pm_url_para").getValue().content_default),
                            pm_keywords: Ext.htmlEncode(Ext.getCmp("pm_keywords").getValue()),
                            pm_description: Ext.htmlEncode(Ext.getCmp("pm_description").getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {

                                Ext.Msg.alert(INFORMATION, SUCCESS);
                                PageMetaStore.load();
                                editWin.close();

                            } else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    });
                }
            }

        }]
    });

    var editWin = Ext.create('Ext.window.Window', {
        title: "網頁元數據編輯",
        id: 'editWin',
        iconCls: row ? "icon-user-edit" : "icon-user-add",
        width: 500,
        height: 260,
        layout: 'fit',
        items: [editFrm],
        constrain: true, //束縛窗口在框架內
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: CLOSEFORM,
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('editWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }],
        listeners: {
            'show': function () {
                if (row) {
                    editFrm.getForm().loadRecord(row); //如果是編輯的話
                    initForm(row);
                }
                else {
                    editFrm.getForm().reset(); //如果是編輯的話
                }
            }
        }
    });
    editWin.show();
    function initForm(Row) {

    }


}