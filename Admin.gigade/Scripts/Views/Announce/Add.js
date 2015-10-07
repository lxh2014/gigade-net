function HtmlChange() {
    if ($('.k-editor-textarea').val().indexOf('<map') >= 0) {
        var b = $('.k-editor-textarea').val().replace(/>\s*<map/g, '><map');
        $('.k-editor-textarea').val(b);
    }
}
editFunction = function (row, store) {
  
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/Announce/AnnounceContentSave',
        defaults: { anchor: "95%", msgTarget: "side" },
        //////編輯器
        listeners: {
            'afterrender': function () {
                jQuery(function () {
                    jQuery('textarea[name=content]').kendoEditor({
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
                                "tooltip": INSERTIMAGE,
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
                                        var htmlsrc = $('textarea[name=content]').data("kendoEditor");// Ext.getCmp('kendoEditor').getValue();
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
                fieldLabel: ID,
                id: 'announce_id',
                name: 'announce_id',
                hidden: true
            },
            {
                xtype: 'textfield',
                fieldLabel: TITLE,
                id: 'title',
                name: 'title',
                allowBlank: false
            },
            {
                xtype: 'combobox',
                allowBlank: true,
                editable: false,
                fieldLabel: ANNOUNCETYPE,
                hidden: false,
                allowBlank: false,
                id: 'type',
                name: 'type',
                store: typestore,
                displayField: 'parameterName',
                valueField: 'parameterCode',
                typeAhead: true,
                value: 0,
                queryMode: 'local',
                forceSelection: false,
                emptyText: SELECT
            },
            {
                xtype: 'numberfield',
                fieldLabel: SORT,
                id: 'sort',
                name: 'sort',
                value: 0,
                minValue: 0
            },

            {
                xtype: 'textarea',
                fieldLabel: ANNOUNCECONTENT,
                id: 'content',
                name: 'content'
            },
            {
                xtype: 'radiogroup',
                id: 'status',
                name: 'status',
                fieldLabel: STATUS,
                defaultType: 'radiofield',
                //  hidden: true,
                defaults: {
                    flex: 1,
                    name: 'status'
                },
                columns: 2,
                vertical: true,
                items: [
                      {
                          boxLabel: SHOW,
                          id: 'show',

                          inputValue: 1
                      },
                        {
                            boxLabel: HIDE,
                            id: 'hide',
                            checked: true,
                            inputValue: 0
                        }
                ]
            }
        ],
        buttons: [
            {
                formBind: true,
                disabled: true,
                text: SAVE,
                handler: function () {
                    if (Ext.htmlEncode(Ext.getCmp('content').getValue()) == "") {
                        Ext.Msg.alert(INFORMATION, ANNOUNCETIP);
                        return;
                    }
                    else {
                        var form = this.up('form').getForm();
                        if (form.isValid()) {
                            this.disable();
                            form.submit({
                                params: {
                                    announce_id: Ext.htmlEncode(Ext.getCmp('announce_id').getValue()),
                                    title: Ext.htmlEncode(Ext.getCmp('title').getValue()),
                                    type: Ext.htmlEncode(Ext.getCmp('type').getValue()),
                                    sort: Ext.htmlEncode(Ext.getCmp('sort').getValue()),
                                    content: Ext.htmlEncode(Ext.getCmp('content').getValue()),
                                    status: Ext.htmlEncode(Ext.getCmp('status').getValue().status)
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(action.response.responseText);
                                    if (result.success) {
                                        Ext.Msg.alert(INFORMATION, SUCCESS);
                                        editWin.close();
                                        store.load();
                                    }
                                    else {
                                        Ext.Msg.alert(INFORMATION, FAILURE);
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
        title: ANNOUNCECONTENT,
        iconCls: 'icon-user-edit',
        id: 'editWin',
        width: 751,
        height: 450,
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
             qtip: ISCLOSE,
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
         }
        ],
        listeners: {
            'show': function () {
                if (row) {
                    editFrm.getForm().loadRecord(row);
                    initRow(row);
                }
                else {
                    editFrm.getForm().reset();
                }
            }
        }
    });
    editWin.show();

    function initRow(row) {
        $('textarea[name=content]').data("kendoEditor").value(row.data.content.replace(/>\s*<map/g, '><map'));
        //   Ext.getCmp('status').show();
        switch (row.data.status) {
            case 1:
                Ext.getCmp('show').setValue(true);
                break;
            case 0:
                Ext.getCmp('hide').setValue(true);
                break;
        }
    }
}
