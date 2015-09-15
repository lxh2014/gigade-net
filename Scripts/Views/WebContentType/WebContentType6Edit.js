editFunction = function (Row, store) {
    var content_status_num = 0; //參數表中預設數量限制
    var list_status_num = 0; //列表中已啟用的數量

    var keeditor = new Ext.form.TextArea({
        id: 'keeditor',
        fieldLabel: '员工描述',
        width: 600,
        height: 200,
        fieldLabel: "商品編號"
    });
    areaidStore.on('beforeload', function () {
        Ext.apply(areaidStore.proxy.extraParams,
               {
                   type: 'area',
                   webcontenttype_page: 'web_content_type6'
               });
    });
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        url: '/WebContentType/SaveWebContentType6',
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 80 },
        listeners: {
            'render': function () {
                KE.show({
                    id: 'keeditor',
                    width: 100,
                    imageUploadJson: '../../../../WebContentType/UploadHtmlEditorPicture'
                });
                setTimeout("KE.create('keeditor');", 1000);
            }
        },
        items: [
            {
                xtype: 'textfield',
                fieldLabel: 'ID',
                id: 'content_id',
                name: 'content_id',
                hidden: true
            },
            {
                xtype: 'combobox', //網頁
                allowBlank: false,
                editable: false,
                typeAhead: true,
                forceSelection: false,
                fieldLabel: "網頁",
                id: 'page_id',
                name: 'page_id',
                hiddenName: 'page_id',
                colName: 'page_id',
                store: pageidStore,
                displayField: 'page_name',
                valueField: 'page_id',
                emptyText: "請選擇",
                listeners: {
                    "select": function (combo, record, index) {
                        var area = Ext.getCmp('area_id');
                        var status = Ext.getCmp('content_status');
                        if (combo.getValue() != undefined && area.getValue() != undefined) {
                            status.setDisabled(false);
                        }
                        pageid = Ext.getCmp('page_id').getValue();
                        area.clearValue();
                        areaidStore.removeAll();
                    }
                }
            },
            {
                xtype: 'combobox', //區域
                allowBlank: false,
                editable: false,
                typeAhead: true,
                forceSelection: false,
                fieldLabel: "區域",
                id: 'area_id',
                name: 'area_id',
                hiddenName: 'area_id',
                colName: 'area_id',
                store: areaidStore,
                displayField: 'area_name',
                valueField: 'area_id',
                emptyText: "請選擇",
                listeners: {
                    beforequery: function (qe) {
                        areaidStore.load();
                    },
                    "select": function (combo, record, index) {
                        var page = Ext.getCmp('page_id');
                        var area = Ext.getCmp('area_id');
                        var status = Ext.getCmp('content_status');
                        if (page.getValue() != undefined && combo.getValue() != undefined) {
                            status.setDisabled(false);
                        }
                        Ext.Ajax.request({
                            url: "/WebContentType/GetLinkUrl",
                            method: 'post',
                            type: 'text',
                            params: {
                                pageid: page.getValue(),
                                areaid: area.getValue(),
                                webcontenttype_page: 'web_content_type6'
                            },
                            success: function (form, action) {
                                var result = Ext.decode(form.responseText);
                                if (result.success) {
                                    msg = result.msg;
                                    Ext.getCmp("link_url").setValue(msg);
                                }
                            }
                        });
                    }
                }
            },
            {
                xtype: 'textfield',
                fieldLabel: "首頁標",
                allowBlank: false,
                maxLength: 50,
                id: 'home_title',
                name: 'home_title'
            },
            {
                xtype: 'filefield',
                name: 'home_image',
                id: 'home_image',
                fieldLabel: "圖檔",
                msgTarget: 'side',
                buttonText: "選擇圖片",
                submitValue: true,
                allowBlank: true,
                fileUpload: true
            },
            {
                xtype: 'radiogroup',
                hidden: false,
                id: 'content_status',
                name: 'content_status',
               
                fieldLabel: "是否啟用",
                colName: 'content_status',
                width: 400,
                defaults: {
                    name: 'ContentStatus',
                    margin: '0 8 0 0'
                },
                columns: 2,
                vertical: true,
                items: [
                { boxLabel: CONTENTSTATUS, id: 'isStatus', inputValue: '1', checked: true
                    //                    listeners: {
                    //                        change: function (radio, newValue, oldValue) {
                    //                            //var targetCol = Ext.getCmp("content_default");
                    //                            if (newValue) {
                    //                                //targetCol.show();
                    //                              
                    //                            }
                    //                        }
                    //                    }
                },
                { boxLabel: NOCONTENTSTATUS, id: 'noStatus', inputValue: '0'
                    //                ,
                    //                    listeners: {
                    //                        change: function (radio, newValue, oldValue) {
                    //                            var targetCol = Ext.getCmp("content_default");
                    //                            var targetCol2 = Ext.getCmp("noDef");
                    //                            if (newValue) {
                    //                                targetCol2.setValue(true);
                    //                                targetCol.hide();
                    //                            }
                    //                        }
                    //                    }
                }
               ]
            },
        //            {
        //                //0=預設(勾選啟用的才能設預設)，最多1筆，非預設=1
        //                xtype: 'radiogroup',
        //                hidden: false,
        //                id: 'content_default',
        //                name: 'content_default',
        //                fieldLabel: "是否預設",
        //                colName: 'content_default',
        //                width: 400,
        //                defaults: {
        //                    name: 'ContentDefault',
        //                    margin: '0 8 0 0'
        //                },
        //                columns: 2,
        //                vertical: true,
        //                items: [
        //                { boxLabel: "預設", id: 'isDef', inputValue: '0' },
        //                { boxLabel: "非預設", id: 'noDef', inputValue: '1', checked: true }
        //               ]
        //            },
            {//自行輸入(連結回吉甲地影音專區的某個影片)
            xtype: 'textfield',
            fieldLabel: LINKURL,
            maxLength: 255,
            allowBlank: false,
            vtype: 'url',
            id: 'link_url',
            name: 'link_url'
        },
            {
                xtype: 'combobox', //開新視窗
                allowBlank: false,
                editable: false,
                fieldLabel: LINKMODE,
                id: 'link_mode',
                name: 'link_mode',
                hiddenName: 'link_mode',
                colName: 'link_mode',
                store: linkModelStore,
                displayField: 'link_mode_name',
                valueField: 'link_mode',
                typeAhead: true,
                queryMode: 'local',
                forceSelection: false,
                value: 1
            },
            {
                xtype: 'textfield',
                fieldLabel: GJZ,
                maxLength: 255,
                allowBlank: false,
                id: 'keywords',
                name: 'keywords'
            },
            {
                xtype: 'textfield',
                fieldLabel: "內頁標",
                maxLength: 255,
                allowBlank: false,
                id: 'content_title',
                name: 'content_title'
            },
            keeditor
        ],
        buttons: [{
            text: SAVE,
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    var oldStatus = 0; //修改時原數據的狀態為不啟用，要修改為啟用時，並且當前啟用值大於等於限制值，並且值存在時才提示
                    if (Row) {
                        oldStatus = Row.data.content_status;
                    }
                    Ext.Ajax.request({
                        url: "/WebContentType/GetDefaultLimit",
                        method: 'post',
                        async: false, //true為異步，false為同步
                        params: {
                            storeType: "web_content_type6",
                            site: "7",
                            page: Ext.getCmp("page_id").getValue(),
                            area: Ext.getCmp("area_id").getValue()
                        },
                        success: function (form, action) {
                            var result = Ext.decode(form.responseText);
                            if (result.success) {
                                list_status_num = result.listNum;
                                content_status_num = result.limitNum;
                            }
                        }
                    });
                    if (Ext.getCmp("content_status").getValue().content_status == "1" && oldStatus == 0 && list_status_num >= content_status_num && content_status_num != 0) {//當選擇啟用並且已啟用數大於或等於最大限制值時提示是否執行
                        Ext.Msg.confirm(CONFIRM, Ext.String.format(STATUSTIP), function (btn) {
                            if (btn == 'yes') {
                                form.submit({
                                    params: {
                                        rowid: Ext.htmlEncode(Ext.getCmp("content_id").getValue()),
                                        page_id: Ext.htmlEncode(Ext.getCmp("page_id").getValue()),
                                        area_id: Ext.htmlEncode(Ext.getCmp("area_id").getValue()),
                                        home_title: Ext.htmlEncode(Ext.getCmp("home_title").getValue()),
                                        content_title: Ext.htmlEncode(Ext.getCmp("content_title").getValue()),
                                        home_image: Ext.htmlEncode(Ext.getCmp("home_image").getValue()),
                                        keywords: Ext.htmlEncode(Ext.getCmp("keywords").getValue()),
                                        //content_default: Ext.htmlEncode(Ext.getCmp("content_default").getValue().ContentDefault),
                                        content_status: Ext.htmlEncode(Ext.getCmp("content_status").getValue().ContentStatus),
                                        link_url: Ext.htmlEncode(Ext.getCmp("link_url").getValue()),
                                        link_mode: Ext.htmlEncode(Ext.getCmp("link_mode").getValue()),
                                        content_html: $("#keeditor").val()
                                    },
                                    success: function (form, action) {
                                        var result = Ext.decode(action.response.responseText);
                                        if (result.success) {
                                            if (result.msg != undefined) {
                                                Ext.Msg.alert(INFORMATION, result.msg);
                                            }
                                            else {
                                                Ext.Msg.alert(INFORMATION, SUCCESS);
                                                store.load();
                                                editWin.close();
                                            }
                                        } else {
                                            Ext.Msg.alert(INFORMATION, FAILURE);
                                        }
                                    },
                                    failure: function () {
                                        Ext.Msg.alert(INFORMATION, FAILURE);
                                    }
                                });
                            }
                            else {
                                return;
                            }
                        });
                    }
                    else {
                        form.submit({
                            params: {
                                rowid: Ext.htmlEncode(Ext.getCmp("content_id").getValue()),
                                page_id: Ext.htmlEncode(Ext.getCmp("page_id").getValue()),
                                area_id: Ext.htmlEncode(Ext.getCmp("area_id").getValue()),
                                home_title: Ext.htmlEncode(Ext.getCmp("home_title").getValue()),
                                content_title: Ext.htmlEncode(Ext.getCmp("content_title").getValue()),
                                home_image: Ext.htmlEncode(Ext.getCmp("home_image").getValue()),
                                keywords: Ext.htmlEncode(Ext.getCmp("keywords").getValue()),
                                //  content_default: Ext.htmlEncode(Ext.getCmp("content_default").getValue().ContentDefault),
                                content_status: Ext.htmlEncode(Ext.getCmp("content_status").getValue().ContentStatus),
                                link_url: Ext.htmlEncode(Ext.getCmp("link_url").getValue()),
                                link_mode: Ext.htmlEncode(Ext.getCmp("link_mode").getValue()),
                                content_html: $("#keeditor").val()
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    if (result.msg != undefined) {
                                        Ext.Msg.alert(INFORMATION, result.msg);
                                    }
                                    else {
                                        Ext.Msg.alert(INFORMATION, SUCCESS);
                                        store.load();
                                        editWin.close();
                                    }
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
            }

        }]
    });

    var editWin = Ext.create('Ext.window.Window', {
        title: WEBCONTENTTYPESIX,
        id: 'editWin',
        iconCls: Row ? "icon-user-edit" : "icon-user-add",
        width: 800,
        height: 530,
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
                if (Row) {
                    editFrm.getForm().loadRecord(Row); //如果是編輯的話
                    initForm(Row);
                }
                else {
                    editFrm.getForm().reset(); //如果是編輯的話
                }
            }
        }
    });
    editWin.show();
    function initForm(row) {
        
        switch (row.data.content_status) {
            case 0:
                Ext.getCmp("noStatus").setValue(true);
                break;
            case 1:
                Ext.getCmp("isStatus").setValue(true);
                break;
        }
        //        switch (row.data.content_default) {
        //            case 0:
        //                Ext.getCmp("isDef").setValue(true);
        //                break;
        //            case 1:
        //                Ext.getCmp("noDef").setValue(true);
        //                break;
        //        }
        var img = row.data.home_image.toString();
        var imgUrl = img.substring(img.lastIndexOf("\/") + 1);
        $("#keeditor").val(Row.data.content_html);
        Ext.getCmp('home_image').setRawValue(imgUrl);
    }
    pageidStore.on('beforeload', function () {
        Ext.apply(pageidStore.proxy.extraParams,
            {
                type: 'page',
                webcontenttype_page: 'web_content_type6'
            });
    });
    pageidStore.load();
    areaidStore.on('beforeload', function () {
        Ext.apply(areaidStore.proxy.extraParams,
            {
                type: 'area',
                pageid: Ext.getCmp('page_id').getValue() == null ? "0" : Ext.getCmp('page_id').getValue(),
                webcontenttype_page: 'web_content_type6'
            });
    });
    areaidStore.load();
}


