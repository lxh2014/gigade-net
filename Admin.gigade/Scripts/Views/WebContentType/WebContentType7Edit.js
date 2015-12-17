editFunction = function (Row, store) {
    var content_status_num = 0; //參數表中預設數量限制
    var list_status_num = 0; //列表中已啟用的數量

    var kindEditor = new Ext.form.TextArea({
        id: 'kindEditor',
        fieldLabel: '员工描述',
        width: 500,
        height: 200,
        fieldLabel: "商品編號"
    });

    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        url: '/WebContentType/SaveWebContentType7',
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 80 },
        listeners: {
            'render': function () {
                KE.show({
                    id: 'kindEditor',
                    width: 100,
                    imageUploadJson: '../../../../WebContentType/UploadHtmlEditorPicture'
                });
                setTimeout("KE.create('kindEditor');", 1000);
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
                                webcontenttype_page: 'web_content_type7'
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
                id: 'home_title',
                name: 'home_title',
                maxLength: "50"
            },
            {
                xtype: 'textareafield',
                fieldLabel: '首頁文<br/>(最多限100字)',
                id: "home_text",
                name: 'home_text',
                maxLength: "100",
                grow: true//,
                //anchor: '100%'
            },
            {
                //圖檔
                xtype: 'filefield',
                name: 'content_image',
                id: 'content_image',
                fieldLabel: "圖檔",
                msgTarget: 'side',
                buttonText: "選擇圖片",
                submitValue: true,
                allowBlank: false,
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
                                var targetCol2 = Ext.getCmp("noDef");
                                if (newValue) {
                                    targetCol2.setValue(true);
                                    targetCol.setDisabled(true);
                                }
                            }
                        }
                    }
               ]
            },
            {
                //0=預設(勾選啟用的才能設預設)，最大4筆，非預設=1
                xtype: 'radiogroup',
               // hidden: false,
                id: 'content_default',
                name: 'content_default',
                fieldLabel: "是否預設",
                colName: 'content_default',
                width: 400,
                defaults: {
                    name: 'content_default',
                    margin: '0 8 0 0'
                },
                columns: 2,
                vertical: true,
                items: [
                    {
                        boxLabel: "預設", id: 'isDef', inputValue: '0'
                    },
                    { boxLabel: "非預設", id: 'noDef', inputValue: '1', checked: true }
               ]
            },
            {
                //自行輸入(連結回吉甲地影音專區的某個影片)
                xtype: 'textfield',
                fieldLabel: LINKURL,
                allowBlank: false,
                vtype: 'url',
                id: 'link_url',
                name: 'link_url'
            },
            {
                xtype: 'combobox', //開新視窗
                allowBlank: false,
                editable: false,
                typeAhead: true,
                forceSelection: false,
                fieldLabel: LINKMODE,
                id: 'link_mode',
                name: 'link_mode',
                hiddenName: 'link_mode',
                colName: 'link_mode',
                store: linkModelStore,
                displayField: 'link_mode_name',
                valueField: 'link_mode',
                queryMode: 'local',
                value: 1
            },
            {
                xtype: 'textfield',
                fieldLabel: GJZ,
                allowBlank: false,
                id: 'keywords',
                name: 'keywords'
            },
            {
                xtype: 'textfield',
                fieldLabel: CONTENTTITLE,
                id: 'content_title',
                name: 'content_title',
                allowBlank: false,
                maxLength: "255"
            },
            {
                xtype: 'numberfield',
                fieldLabel: '排序',
                allowBlank: false,
                id: 'sort',
                name: 'sort',
                minValue: 0,
                value:0,
                maxValue: 10000
            },
            {
                xtype: 'datetimefield',
                fieldLabel: '上架時間',
                allowBlank: true,
                id: 'start_time',
                name: 'start_time',
                format: 'Y-m-d H:i:s',
                value: Tomorrow(),
                time: { hour: 00, min: 00, sec: 00 },
                listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("start_time");
                        var end = Ext.getCmp("end_time");
                        if (end.getValue() < start.getValue()) {
                            var start_date = start.getValue();
                            Ext.getCmp('end_time').setValue(new Date(start_date.getFullYear(), start_date.getMonth() + 1, start_date.getDate(), 23, 59, 59));
                        }
                    }
                }
            },
            {
                xtype: 'datetimefield',
                fieldLabel: '下架時間',
                allowBlank: true,
                time: { hour: 23, min: 59, sec: 59 },
                id: 'end_time',
                name: 'end_time',
                format: 'Y-m-d H:i:s',
                value: setNextMonth(Tomorrow(), 1),
                listeners: {
                        select: function (a, b, c) {
                            var start = Ext.getCmp("start_time");
                            var end = Ext.getCmp("end_time");
                            if (end.getValue() < start.getValue()) {
                                var end_date = end.getValue();
                                Ext.getCmp('start_time').setValue(new Date(end_date.getFullYear(), end_date.getMonth() - 1, end_date.getDate()));
                            }
                        }
                }
            },
            kindEditor,
        ],
        buttons: [{
            text: SAVE,
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                var start = Ext.getCmp("start_time");
                var end = Ext.getCmp("end_time");
                if (end.getValue() < start.getValue()) {
                    alert("上架時間不能大於下架時間！");
                }
                else
                {
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
                                storeType: "web_content_type7",
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
                        if (oldStatus == 0 && Ext.getCmp("content_status").getValue().content_status == "1" && list_status_num >= content_status_num && content_status_num != 0) {//當選擇啟用並且已啟用數大於或等於最大限制值時提示是否執行
                            Ext.Msg.confirm(CONFIRM, Ext.String.format(STATUSTIP), function (btn) {
                                if (btn == 'yes') {
                                    form.submit({
                                        params: {
                                            rowid: Ext.htmlEncode(Ext.getCmp("content_id").getValue()),
                                            page_id: Ext.htmlEncode(Ext.getCmp("page_id").getValue()),
                                            area_id: Ext.htmlEncode(Ext.getCmp("area_id").getValue()),
                                            keywords: Ext.htmlEncode(Ext.getCmp("keywords").getValue()),
                                            home_title: Ext.htmlEncode(Ext.getCmp("home_title").getValue()),
                                            home_text: Ext.htmlEncode(Ext.getCmp("home_text").getValue()),
                                            content_title: Ext.htmlEncode(Ext.getCmp("content_title").getValue()),
                                            sort: Ext.htmlEncode(Ext.getCmp("sort").getValue()),
                                            start_time: Ext.htmlEncode(Ext.getCmp("start_time").getValue()),
                                            end_time: Ext.htmlEncode(Ext.getCmp("end_time").getValue()),
                                            content_html: $("#kindEditor").val()
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
                                    keywords: Ext.htmlEncode(Ext.getCmp("keywords").getValue()),
                                    home_title: Ext.htmlEncode(Ext.getCmp("home_title").getValue()),
                                    home_text: Ext.htmlEncode(Ext.getCmp("home_text").getValue()),
                                    content_title: Ext.htmlEncode(Ext.getCmp("content_title").getValue()),
                                    sort: Ext.htmlEncode(Ext.getCmp("sort").getValue()),
                                    start_time: Ext.htmlEncode(Ext.getCmp("start_time").getValue()),
                                    end_time: Ext.htmlEncode(Ext.getCmp("end_time").getValue()),
                                    content_html: $("#kindEditor").val()
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
            }
        }]
    });

    var editWin = Ext.create('Ext.window.Window', {
        title: WEBCONTENTTYPESEVEN,
        id: 'editWin',
        iconCls: Row ? "icon-user-edit" : "icon-user-add",
        width: 800,
        height: 640,
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
                } else {
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
        switch (row.data.content_default) {
            case 0:
                Ext.getCmp("isDef").setValue(true);
                break;
            case 1:
                Ext.getCmp("noDef").setValue(true);
                break;
        }
        var img = row.data.content_image.toString();
        var imgUrl = img.substring(img.lastIndexOf("\/") + 1);
        $("#kindEditor").val(row.data.content_html);
        Ext.getCmp('content_image').setRawValue(imgUrl);
    }

    pageidStore.on('beforeload', function () {
        Ext.apply(pageidStore.proxy.extraParams,
                {
                    type: 'page',
                    webcontenttype_page: 'web_content_type7'
                });
    });
    pageidStore.load();

    areaidStore.on('beforeload', function () {
        Ext.apply(areaidStore.proxy.extraParams,
                {
                    type: 'area',
                    pageid: Ext.getCmp('page_id').getValue() == null ? "0" : Ext.getCmp('page_id').getValue(),
                    webcontenttype_page: 'web_content_type7'
                });
    });
    areaidStore.load();
}
function setNextMonth(source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    }
    else if (n > 0) {
        s.setHours(23, 59, 59);
    }
    return s;
}
