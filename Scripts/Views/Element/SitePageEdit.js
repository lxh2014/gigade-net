/* 
* 文件名稱 :SitePageEdit.js 
* 文件功能描述 :頁面管理畫面 
* 版權宣告 : 
* 開發人員 : changjian0408j 
* 版本資訊 : 1.0 
* 日期 : 2014/10/14 
* 修改人員 : 
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/
editFunction = function (row, store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        //defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/Element/SaveSitePage',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'textfield',
                fieldLabel: PAGEID,
                id: 'page_id',
                name: 'page_id',
                submitValue: true,
                hidden: true
            },
            {
                xtype: 'textfield',
                name: 'page_name',
                id: 'page_name',
                maxLength: 25,
                allowBlank: false,
                submitValue: true,
                fieldLabel: PAGENAME
            },
            {
                xtype: 'textfield',
                name: 'page_url',
                id: 'page_url',
                //vtype: 'url',
                submitValue: true,
                allowBlank: true,
                fieldLabel: PAGEURL
            },
            {
                xtype: 'textarea',
                name: 'page_html',
                id: 'page_html',
                maxLength: 100,
                allowBlank: true,
                submitValue: true,
                fieldLabel: PAGEHTML
            },
            {
                xtype: 'textarea',
                name: 'page_desc',
                id: 'page_desc',
                maxLength: 100,
                allowBlank: true,
                submitValue: true,
                fieldLabel: PAGEDESC
            }
        ],
        buttons: [
            {
                formBind: true,
                disabled: true,
                text: SAVE,
                handler: function () {
                    var form = this.up('form').getForm();
                    if (form.isValid()) {
                        form.submit({
                            params: {
                                page_id: Ext.htmlEncode(Ext.getCmp('page_id').getValue()),
                                page_name: Ext.htmlEncode(Ext.getCmp('page_name').getValue()),
                                page_url: Ext.htmlEncode(Ext.getCmp('page_url').getValue()),
                                page_html: Ext.htmlEncode(Ext.getCmp('page_html').getValue()),
                                page_desc: Ext.htmlEncode(Ext.getCmp('page_desc').getValue())
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    Ext.Msg.alert(INFORMATION, SUCCESS);
                                    SitePageListStore.load();
                                    editWin.close();
                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, FAILURE);
                                    SitePageListStore.load();
                                    editWin.close();
                                }
                            },
                            failure: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                Ext.Msg.alert(INFORMATION, FAILURE);
                                SitePageListStore.load();
                                editWin.close();
                            }
                        });
                    }
                }
            }
        ]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: SAVEORUPDATE,
        iconCls: row ? 'icon-user-edit' : 'icon-user-add',
        id: 'editWin',
        width: 400,
        y: 100,
        layout: 'fit',
        items: [editFrm],
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
                qtip: ISCLOSE,
                handler: function (event, toolEl, panel) {
                    Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                        if (btn == "yes") {
                            Ext.getCmp('editWin').destroy();
                        } else {
                            return false;
                        }
                    });
                }
            }
        ],
        listeners: {
            'show': function () {
                editFrm.getForm().loadRecord(row);
            }
        }
    });
    editWin.show();
    initForm(row);
}