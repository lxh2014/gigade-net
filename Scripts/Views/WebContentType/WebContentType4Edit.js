editFunction = function (Row, store) {
    var content_status_num = 0; //參數表中預設數量限制
    var list_status_num = 0; //列表中已啟用的數量


    var keeditor = new Ext.form.TextArea({
        id: 'keeditor',
        fieldLabel: HOMETEXT,
        width: 600,
        height: 200
    });
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        url: '/WebContentType/SaveWebContentType4',
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
                fieldLabel: PAGEID,
                id: 'page_id',
                name: 'page_id',
                hiddenName: 'page_id',
                colName: 'page_id',
                store: pageidStore,
                displayField: 'page_name',
                valueField: 'page_id',
                emptyText: SELECT,
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
                fieldLabel: AREAID,
                id: 'area_id',
                name: 'area_id',
                hiddenName: 'area_id',
                colName: 'area_id',
                store: areaidStore,
                displayField: 'area_name',
                valueField: 'area_id',
                emptyText: SELECT,
                listeners: {
                    beforequery: function (qe) {
                        //                        delete qe.combo.lastQuery;
                        //                        areaidStore.on('beforeload', function () {
                        //                            Ext.apply(areaidStore.proxy.extraParams,
                        //                                        {
                        //                                            type: 'area',
                        //                                            pageid: pageid,
                        //                                            webcontenttype_page: 'web_content_type4'
                        //                                        });
                        //                        });
                        areaidStore.load();
                    },
                    "select": function (combo, record, index) {
                        var page = Ext.getCmp('page_id');
                        var status = Ext.getCmp('content_status');
                        var area = Ext.getCmp('area_id');
                        var brand = Ext.getCmp('brand_id');
                        if (page.getValue() != undefined && combo.getValue() != undefined) {
                            status.setDisabled(false);
                        }
                        if (page.getValue() != undefined && area.getValue() != undefined) {
                            brand.setDisabled(false);
                        }
                    }
                }

            },
                {
                    xtype: 'combobox', //品牌
                    allowBlank: false,
                    editable: false,
                    //typeAhead: true,
                    disabled: true,
                    //forceSelection: false,
                    fieldLabel: BRANDID,
                    id: 'brand_id',
                    name: 'brand_name',
                    hiddenName: 'brand_id',
                    colName: 'brand_id',
                    store: BrandStore,
                    displayField: 'brand_name',
                    valueField: 'brand_id',
                    emptyText: SELECT,
                    listeners: {
                        beforequery: function (qe) {
                            BrandStore.on('beforeload', function () {
                                Ext.apply(BrandStore.proxy.extraParams,
                                    {
                                        webcontenttype: 'web_content_type4', content_id: Row != null ? Row.data.content_id : 0
                                    });
                            });
                        },
                        "select": function (combo, record, index) {
                            var page = Ext.getCmp('page_id');
                            var area = Ext.getCmp('area_id');
                            var brand = Ext.getCmp('brand_id');
                            Ext.Ajax.request({
                                url: "/WebContentType/GetLinkUrl",
                                method: 'post',
                                type: 'text',
                                params: {
                                    brandid: brand.getValue(),
                                    pageid: page.getValue(),
                                    areaid: area.getValue(),
                                    webcontenttype_page: 'web_content_type4'
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
                    xtype: 'radiogroup',
                    hidden: false,
                    id: 'content_status',
                    name: 'content_status',
                    fieldLabel: ISSTATUS,
                    colName: 'content_status',
                    width: 400,
                    defaults: {
                        name: 'content_status',
                        margin: '0 8 0 0'
                    },
                    columns: 2,
                    vertical: true,
                    items: [
                { boxLabel: CONTENTSTATUS, id: 'isStatus', inputValue: '1', checked: true,
                    listeners: {
                        change: function (radio, newValue, oldValue) {
                            var targetCol = Ext.getCmp("content_default");
                            if (newValue) {
                                targetCol.setDisabled(false);

                            }
                        }
                    }
                },
                { boxLabel: NOCONTENTSTATUS, id: 'noStatus', inputValue: '0',
                    listeners: {
                        change: function (radio, newValue, oldValue) {
                            var targetCol = Ext.getCmp("content_default");
                            var yes = Ext.getCmp("isDef");
                            var no = Ext.getCmp("noDef");
                            var noStatus = Ext.getCmp("noStatus");
                            if (newValue) {
                                targetCol.setDisabled(true);
                            }
                            if (noStatus) {
                                yes.setValue(false);
                                no.setValue(true);
                            }
                        }
                    }
                }
               ]
                },
           {//0=預設(勾選啟用的才能設預設)，最多1筆，非預設=1
               xtype: 'radiogroup',
               // hidden: false,
               id: 'content_default',
               name: 'content_default',
               fieldLabel: ISDEFAULT,
               colName: 'content_default',
               width: 400,
               defaults: {
                   name: 'content_default',
                   margin: '0 8 0 0'
               },
               columns: 2,
               vertical: true,
               items: [
                { boxLabel: CONTENTDEFAULT, id: 'isDef', inputValue: '0'

                },
                { boxLabel: NOCONTENTDEFAULT, id: 'noDef', inputValue: '1', checked: true
                }
               ]
           },
           {//自行輸入(連結回吉甲地影音專區的某個影片)
               xtype: 'textfield',
               fieldLabel: LINKURL,
               allowBlank: false,
               vtype: 'url',
               id: 'link_url',
               name: 'link_url',
               maxLength: "255"

           },
        //           {//在那個頁面顯示(如food.php,life.php)，空值代表不設限
        //               xtype: 'textfield',
        //               fieldLabel: "顯示頁面",
        //               allowBlank: false,
        //               id: 'link_page',
        //               name: 'link_page'

        //           },
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
                            storeType: "web_content_type4",
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
                                        brand_id: Ext.htmlEncode(Ext.getCmp("brand_id").getValue()),
                                        content_status: Ext.htmlEncode(Ext.getCmp("content_status").getValue().content_status),
                                        content_default: Ext.htmlEncode(Ext.getCmp("content_default").getValue().content_default),
                                        link_url: Ext.htmlEncode(Ext.getCmp("link_url").getValue()),
                                        link_mode: Ext.htmlEncode(Ext.getCmp("link_mode").getValue()),
                                        home_text: $("#keeditor").val()
                                        //                            type_id: Ext.htmlEncode(Ext.getCmp("type_id").getValue())

                                    },
                                    success: function (form, action) {
                                        var result = Ext.decode(action.response.responseText);
                                        if (result.success) {
                                            Ext.Msg.alert(INFORMATION, SUCCESS);
                                            store.load();
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
                                brand_id: Ext.htmlEncode(Ext.getCmp("brand_id").getValue()),
                                content_status: Ext.htmlEncode(Ext.getCmp("content_status").getValue().content_status),
                                content_default: Ext.htmlEncode(Ext.getCmp("content_default").getValue().content_default),
                                link_url: Ext.htmlEncode(Ext.getCmp("link_url").getValue()),
                                link_mode: Ext.htmlEncode(Ext.getCmp("link_mode").getValue()),
                                home_text: $("#keeditor").val()
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    Ext.Msg.alert(INFORMATION, SUCCESS);
                                    store.load();
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
            }
        }]
    });

    var editWin = Ext.create('Ext.window.Window', {
        title: WEBCONTENTTYPEFOUR,
        id: "editWin",
        iconCls: Row ? "icon-user-edit" : "icon-user-add",
        width: 800,
        height: 470,
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
            //            beforeclose: function (panel, eOpts) {
            //                if (!confirm("是否確定關閉窗口？")) {
            //                    return false;
            //                }
            //            },
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

        if (row.data.brand_id != 0) {
            Ext.getCmp("brand_id").setDisabled(false);
        }

        switch (row.data.content_status) {
            case 0:
                Ext.getCmp("noStatus").setValue(true);
                break;
            case 1:
                Ext.getCmp("isStatus").setValue(true);
                break;
        }
        switch (row.data.content_default) {
            case 0:
                Ext.getCmp("isDef").setValue(true);
                break;
            case 1:
                Ext.getCmp("noDef").setValue(true);
                break;
        }
        $("#keeditor").val(Row.data.content_html);
        Ext.getCmp('brand_id').setRawValue(Row.data.brand_name);
        //        var img = row.data.content_image.toString();
        //        var imgUrl = img.substring(img.lastIndexOf("\/") + 1);

        //        Ext.getCmp('content_image').setRawValue(imgUrl);
    }

    pageidStore.on('beforeload', function () {
        Ext.apply(pageidStore.proxy.extraParams,
                {
                    type: 'page',
                    webcontenttype_page: 'web_content_type4'
                });
    });
    pageidStore.load();

    areaidStore.on('beforeload', function () {
        Ext.apply(areaidStore.proxy.extraParams,
                {
                    type: 'area',
                    pageid: Ext.getCmp('page_id').getValue() == null ? "0" : Ext.getCmp('page_id').getValue(),
                    webcontenttype_page: 'web_content_type4'
                });
    });
    areaidStore.load();
}


