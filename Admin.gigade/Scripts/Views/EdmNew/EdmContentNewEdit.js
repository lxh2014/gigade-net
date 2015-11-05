editFunction = function (row, store) {
    var split_str = document.getElementById('split_str').value;
    Ext.define('gigade.edm_group_new', {
        extend: 'Ext.data.Model',
        fields: [
            { name: 'group_id', type: 'int' },
            { name: 'group_name', type: 'string' }
        ]
    });
    var EdmGroupNewStore = Ext.create("Ext.data.Store", {
        autoLoad: true,
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
    var MailSenderStore = Ext.create("Ext.data.Store", {
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
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
       // height:1000,
        url: '/EdmNew/SaveEdmContentNew',
        defaults: { anchor: "95%", msgTarget: "side" },
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
                lastQuery: ''
            },
            {
                xtype: 'combobox',
                fieldLabel: '郵件重要度',
                store: importanceStore,
                displayField: 'txt',
                valueField: 'value',
                id: 'importance',
                name: 'importance',
                value: 1,
                editable: false,
                lastQuery: ''
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
                lastQuery: '',
                editable: false,
                listeners: {
                    'select': function () {
                        var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                        myMask.show();
                        Ext.Ajax.request({
                            url: '/EdmNew/GetEditUrlData',
                            params: {
                                edit_url: Ext.getCmp('template_id').getRawValue(),
                            },
                            success: function (data) {
                                myMask.hide();
                                var result = data.responseText;
                                if (result == "獲取網頁出現異常！") {
                                    Ext.Msg.alert("提示信息", "獲取網頁出現異常！");
                                    }
                                else {
                                    var index = result.indexOf(split_str);
                                    if(index > 0) {
                                        Query(2);
                                        var editData1 = result.substr(0, index);
                                        var editData2 = result.substr(index + split_str.length, result.length - (index + split_str.length));
                                        $("#editor").data("kendoEditor").value(editData1);
                                        $("#editor2").data("kendoEditor").value(editData2);
                                    }
                                    else {
                                        Query(1);
                                        $("#editor3").data("kendoEditor").value(result);
                                    }
                                  
                                }
                            },
                            failure: function () {
                                myMask.hide();
                                Ext.Msg.alert("提示信息", "獲取網頁出現異常！");
                            }
                        });
                    }
                }
            },
 
            {
                xtype: 'textarea',
                html: '<div id="template_area"></div>',
                frame: true,
                border: false
            }
        ],
        buttons: [
            {
                formBind: true,
                disabled: true,
                text: '保存',
                handler: function () {
                        var form = this.up('form').getForm();
                        var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                        myMask.show();
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
                                  
                                    editor1: document.getElementById('editor').value,
                                    editor2: document.getElementById('editor2').value,
                                    split_str: split_str,
                                },
                                success: function (form, action) {
                                    myMask.hide();
                                    var result = Ext.decode(action.response.responseText);
                                    if (result.success) {
                                        myMask.hide();
                                        Ext.Msg.alert("提示信息", "保存成功! ");
                                        store.load();
                                        editWin.close();
                                    }
                                    else {
                                        myMask.hide();
                                        Ext.Msg.alert("提示信息", "保存失敗! ");
                                        store.load();
                                        editWin.close();
                                    }
                                },
                                failure: function () {
                                    myMask.hide();
                                    Ext.Msg.alert("提示信息", "出現異常! ");
                                }
                            });
                        }

                }
            }
        ]
    });

    var editWin = Ext.create('Ext.window.Window', {
        title: '電子報新增/編輯',
        iconCls: 'icon-user-edit',
        id: 'editWin',
        height: 550,
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
       
        var result = row.data.template_data;
        var index = result.indexOf(split_str);
        if (index > 0) {
            Query(2);
            var editData1 = result.substr(0, index);
            var editData2 = result.substr(index + split_str.length, result.length - (index + split_str.length));
            $("#editor").data("kendoEditor").value(editData1);
            $("#editor2").data("kendoEditor").value(editData2);
        }
        else {
            Query(1);
            $("#editor3").data("kendoEditor").value(result);
        }
    }
    function Query(x) {
        var url ;
        if (x == 1) {
            url = "/EdmNew/Editkendo";
        }
        else {
            url = "/EdmNew/Editkendo2";
        }
        $.ajax({
            async: false,
            url:url,
        }).success(function (partialView) {
            $('#template_area').empty().append(partialView);
            // 呼叫 API 取得 edm_content.template_data
            $('.content_editor').kendoEditor({
                tools: [
                      "bold", "italic", "underline", "strikethrough",
                      "justifyLeft", "justifyCenter", "justifyRight", "justifyFull",
                      "insertUnorderedList", "insertOrderedList", "indent", "outdent",
                      "createLink", "unlink", "insertFile", "insertImage",
                      "subscript", "superscript", "createTable", "foreColor",
                      "addRowAbove", "addRowBelow", "addColumnLeft", "addColumnRight",
                      "deleteRow", "deleteColumn", "viewHtml", "backColor",
                      "fontName", "fontSize", "formatting", "cleanFormatting"
                ],
                stylesheets: [
                    "/images/newsletters/swat/kendo/css/editorStyles.css"
                ]

            });
        });
    }
}