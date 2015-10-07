editFunction = function (row, store) {

    Ext.define('gigade.edm_group_new', {
        extend: 'Ext.data.Model',
        fields: [
            { name: 'group_id', type: 'int' },
            { name: 'group_name', type: 'string' }
        ]
    });
    var EdmGroupNewStore = Ext.create("Ext.data.Store", {
       autoLoad:true,
        model: 'gigade.edm_group_new',
        proxy: {
            type: 'ajax',
            url: '/EdmNew/GetEdmGroupNewStore',
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });
 

    Ext.define('gigade.mailSender', {
        extend: 'Ext.data.Model',
        fields: [
            { name: 'sender_id', type: 'int' },
            { name: 'sender_email', type: 'string' }
        ]
    });
    var MailSenderStore= Ext.create("Ext.data.Store", {
        autoLoad: true,
        model: 'gigade.mailSender',
        proxy: {
            type: 'ajax',
            url: '/EdmNew/GetMailSenderStore',
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });

    Ext.define('gigade.edm_template', {
        extend: 'Ext.data.Model',
        fields: [
            { name: 'template_id', type: 'int' },
            { name: 'edit_url', type: 'string' }
        ]
    });
    var EdmTemplateStore = Ext.create("Ext.data.Store", {
        autoLoad: true,
        model: 'gigade.edm_template',
        proxy: {
            type: 'ajax',
            url: '/EdmNew/GetEdmTemplateStore',
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });

    var importanceStore = Ext.create('Ext.data.Store', {
        fields: ['txt', 'value'],
        data: [
              { 'txt': '一般', 'value': '0' },
              { 'txt': '重要', 'value': '1' },
              { 'txt': '特級', 'value': '2' },
        ]
    });
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        //autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/EdmNew/SaveEdmContentNew',
        defaults: { anchor: "95%", msgTarget: "side" },
        //////編輯器
        listeners: {
            'afterrender': function () {
                jQuery(function () {
                    jQuery('textarea[name=kendoEditor]').kendoEditor({
                        "tools": [
                            { "name": "bold" },
                            { "name": "italic" },
                            { "name": "underline" },
                            { "name": "strikethrough" },
                            { "name": "justifyLeft" },
                            { "name": "justifyCenter" },
                            { "name": "justifyRight" },
                            { "name": "justifyFull" },
                            { "name": "insertUnorderedList" },
                            { "name": "insertOrderedList" },
                            { "name": "outdent" },
                            { "name": "indent" },
                            {
                                "name": "cLink",
                                "tooltip": "插入鏈接",
                                "exec":
                                    function (e) {
                                        var editor = $(this).data("kendoEditor");
                                        editor.exec("createLink");
                                        $('.k-overlay').css('z-index', 19012);//设置遮罩zindex
                                        $('.k-window').css('z-index', 19013);//设置文件选择框zindex,比上面遮罩大1即可
                                    }

                            },
                            { "name": "unlink" },
                            {
                                "name": "iFile",
                                "tooltip": "插入文件",
                                "exec":
                                   function (e) {
                                       var editor = $(this).data("kendoEditor");
                                       editor.exec("insertFile");
                                       $('.k-overlay').css('z-index', 19012);//设置遮罩zindex
                                       $('.k-window').css('z-index', 19013);//设置文件选择框zindex,比上面遮罩大1即可
                                   }
                            },
                            {
                                "name": "iImage",
                                "tooltip": "插入圖片",
                                "exec":
                                    function (e) {
                                        var editor = $(this).data("kendoEditor");
                                        editor.exec("insertImage");
                                        url = $('#k-editor-image-url');
                                        $('.k-selectable').attr("onclick", "selectionChange()");
                                        $('.k-overlay').css('z-index', 19012);//设置遮罩zindex
                                        $('.k-window').css('z-index', 19013);//设置文件选择框zindex,比上面遮罩大1即可
                                    }
                            },
                             {
                                 "name": "preview",
                                 "tooltip": "預覽",
                                 "exec":
                                    function (e) {
                                        var editor = $(this).data("kendoEditor");
                                        var htmlsrc = $('textarea[name=kendoEditor]').data("kendoEditor");// Ext.getCmp('kendoEditor').getValue();
                                        var A = 1000;
                                        var B = 700;
                                        var C = (document.body.clientWidth - A) / 2;
                                        var D = window.open('', null, 'toolbar=yes,location=no,status=yes,menubar=yes,scrollbars=yes,resizable=yes,width=' + A + ',height=' + B + ',left=' + C);
                                        var E = "<html><head><title>預覽</title></head><style>body{line-height:200%;padding:50px;}</style><body><div >" + htmlsrc.body.innerHTML + "</div></body></html>";
                                        D.document.write(E);
                                        D.document.close();
                                    }
                             },
                            { "name": "subscript" },
                            { "name": "superscript" },
                            { "name": "createTable" },
                            { "name": "addColumnLeft" },
                            { "name": "addColumnRight" },
                            { "name": "addRowAbove" },
                            { "name": "addRowBelow" },
                            { "name": "deleteRow" },
                            { "name": "deleteColumn" },
                            {
                                "name": "vHtml",
                                "tooltip": "查看HTML",
                                "exec":
                               function (e) {
                                   var editor = $(this).data("kendoEditor");
                                   editor.exec("viewHtml");
                                   $('.k-editor-textarea').attr("onchange", "HtmlChange()");
                                   $('.k-overlay').css('z-index', 19012);//设置遮罩zindex
                                   $('.k-window').css('z-index', 19013);//设置文件选择框zindex,比上面遮罩大1即可
                               }
                            },
                            { "name": "formatting" },
                            { "name": "fontName" },
                            { "name": "fontSize" },
                            { "name": "foreColor" },
                            { "name": "backColor" }
                        ],
                        "imageBrowser": {
                            "transport": {
                                "read": { "url": "/ImageBrowser/Read" },
                                "type": "imagebrowser-aspnetmvc",
                                "thumbnailUrl": "/ImageBrowser/Thumbnail",
                                "uploadUrl": "/ImageBrowser/Upload",
                                "destroy": { "url": "/ImageBrowser/Destroy" },
                                "create": { "url": "/ImageBrowser/Create" },
                                //  "imageUrl": "http://192.168.16.118/uploads/{0}"
                                "imageUrl": document.getElementById('BaseAddress').value + "/" + "" + document.getElementById('path').value + "" + "/" + "{0}"
                            }
                        }
                    });
                });
            }

        },
        /////編輯器
        items: [
              {
                  xtype: 'displayfield',
                  fieldLabel: '編號',
                  id: 'content_id',
                  name: 'content_id',
                  hidden: true
              },
            {
                xtype: 'combobox',
                store: MailSenderStore,
                valueField: 'sender_id',
                displayField: 'sender_email',
                fieldLabel: '寄件者',
                id: 'sender_id',
                name: 'sender_id',
                queryModel: 'local',
                lastQuery: '',
                editable: false,
                allowBlank: false,
                lastQuery: '',
             
            },
            {
                xtype: 'combobox',
                fieldLabel: '電子報類型',
                store: EdmGroupNewStore,
                displayField: 'group_name',
                valueField: 'group_id',
                allowBlank: false,
                //lastQuery: '',
                id: 'group_id',
                name: 'group_id',
                editable: false,
                lastQuery: '',
              
            },
            {
                xtype: 'combobox',
                fieldLabel: '郵件重要度',
                store: importanceStore,
                displayField: 'txt',
                valueField:'value',
                id: 'importance',
                name: 'importance',
                value:1,
                editable: false,
                lastQuery: '',
            },
            {
                xtype: 'textfield',
                fieldLabel: '郵件主旨',
                id: 'subject',
                name: 'subject',
                allowBlank: false
            },
            {
                xtype: 'combobox',
                store: EdmTemplateStore,
                valueField: 'template_id',
                displayField: 'edit_url',
                fieldLabel: '郵件範本',
                id: 'template_id',
                name: 'template_id',
                editable: false,
                lastQuery:'',
                editable: false,
                listeners: {
                    'select': function () {
                        Ext.Ajax.request({
                            url: '/EdmNew/GetEditUrlData',
                            params: {
                                edit_url: Ext.getCmp('template_id').getRawValue(),
                            },
                            success: function (data) {
                              $('textarea[name=kendoEditor]').data("kendoEditor").value(Ext.util.Format.htmlDecode(data.responseText));
                               
                            },
                            failure: function () {
                                Ext.Msg.alert("提示信息","獲取網頁出現異常！");
                            }
                        });
                    }
                }
            },
            {
                xtype: 'textarea',
                fieldLabel: '郵件內容',
                id: 'kendoEditor',
                name: 'kendoEditor',
            },
        ],
        buttons: [
            {
                formBind: true,
                disabled: true,
                text: '保存',
                handler: function () {
                    if (Ext.htmlEncode(Ext.getCmp('kendoEditor').getValue()) == "") {
                        Ext.Msg.alert("提示信息", '郵件內容為空');
                        return;
                    }
                    else {
                        var form = this.up('form').getForm();
                        if (form.isValid()) {
                            this.disable();
                            form.submit({
                                params: {
                                    content_id: Ext.htmlEncode(Ext.getCmp('content_id').getValue()),
                                    sender_id: Ext.htmlEncode(Ext.getCmp('sender_id').getValue()),
                                    group_id: Ext.htmlEncode(Ext.getCmp('group_id').getValue()),
                                    importance: Ext.htmlEncode(Ext.getCmp('importance').getValue()),
                                    subject: Ext.htmlEncode(Ext.getCmp('subject').getValue()),
                                    template_id: Ext.htmlEncode(Ext.getCmp('template_id').getValue()),
                                    template_data: Ext.htmlEncode(Ext.getCmp('kendoEditor').getValue()),
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(action.response.responseText);
                                    if (result.success) {
                                        Ext.Msg.alert("提示信息", "保存成功! ");
                                        store.load();
                                        editWin.close();
                                    }
                                    else {
                                        Ext.Msg.alert("提示信息", "保存失敗! ");
                                        store.load();
                                        editWin.close();
                                    }
                                }
                            });
                        }
                    }

                }
            }
        ]
    });

    var editWin = Ext.create('Ext.window.Window', {
        title: '電子報新增/編輯',
        iconCls: 'icon-user-edit',
        id: 'editWin',
        height: 650,
        width: 750,
        y: 100,
        layout: 'fit',
        items: [editFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        //  resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: '是否關閉',
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm("確認", "是否確定關閉窗口?", function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('editWin').destroy();
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
                    Ext.getCmp('sender_id').allowBlank = true;
                    Ext.getCmp('group_id').allowBlank = true;
                    Ext.getCmp('template_id').allowBlank = true;
                    editFrm.getForm().loadRecord(row);
                    initRow(row);
                }
                else {
                    Ext.getCmp('sender_id').allowBlank = false;
                    Ext.getCmp('group_id').allowBlank = false;
                    Ext.getCmp('template_id').allowBlank = false;
                    editFrm.getForm().reset();
                }
            }
        }
    });
    editWin.show();
    function initRow(row) {
       $('textarea[name=kendoEditor]').data("kendoEditor").value(row.data.template_data);
    }
}