
function editFunction(RowID, Store) {
    var ID = null;
    if (RowID != null) {
        ID = RowID.data["article_id"];
    }
    function selectionChange() {
        var lis = $('.k-selectable').children();
        for (var i = 0; i < lis.length; i++) {
            var current = $(lis[i]);
            var selected = current.attr("aria-selected");
            if (selected == "true") {
                var dType = current.attr("data-type");
                if (dType == "d") {
                    url.val('http://');
                    return;
                }
                else if (dType == "f") {
                    url.val(decodeURIComponent(url.val()));
                    break;
                }
            }
        }
    }

    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        url: '/Vote/SaveVoteArticle',
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 80 },
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
                                "tooltip": KENDOLINK,
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
                                "tooltip": KENDOFILE,
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
                                "tooltip": INSERTIMG,
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
                                "tooltip": KENDOHTML,
                                "exec":
                               function (e) {
                                   var editor = $(this).data("kendoEditor");
                                   editor.exec("viewHtml");
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
        items: [
            {
                xtype: 'combobox',
                fieldLabel: "活動編號",
                allowBlank: true,
                editable: false,
                hidden: false,
                id: 'event_id',
                name: 'event_id',
                lastQuery: '',
                store: DDRStore,
                displayField: 'event_name',
                valueField: 'event_id',
                typeAhead: true,
                forceSelection: false,
                labelWidth: 80,
                emptyText: 'SELECT'
            },
            {
                xtype: 'textfield',
                allowBlank: true,
                fieldLabel: "會員編號",
                labelWidth: 80,
                id: 'user_id',
                name: 'user_id'
            },
            {
                xtype: 'textfield',
                allowBlank: true,
                fieldLabel: "文章標題",
                labelWidth: 80,
                id: 'article_title',
                name: 'article_title'
            },
            {//專區Banner                
                xtype: 'filefield',
                name: 'article_banner',
                id: 'article_banner',
                allowBlank: true,
                fieldLabel: "文章大圖",
                msgTarget: 'side',
                buttonText: '選擇...',
                submitValue: true,
                labelWidth: 80
            },
            {
                xtype: 'textareafield',
                fieldLabel: "文章內容",
                allowBlank: true,
                id: 'kendoEditor',
                name: 'kendoEditor'
            }
        ],
        buttons: [
        {
            text: '保存',
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    if (Ext.htmlEncode(Ext.getCmp("kendoEditor").getValue()) == "") {
                        Ext.Msg.alert(INFORMATION, KENDOTIP);
                        return;
                    }
                    if (Ext.htmlEncode(Ext.getCmp("kendoEditor").getValue()).length > 5000) {
                        Ext.Msg.alert(INFORMATION, ELECONTENTTIP);
                        return;
                    }
                    form.submit({
                        params: {
                            id: ID,
                            event_id: Ext.htmlEncode(Ext.getCmp("event_id").getValue()),
                            user_id: Ext.htmlEncode(Ext.getCmp("user_id").getValue()),
                            article_title: Ext.htmlEncode(Ext.getCmp("article_title").getValue()),
                            article_banner: Ext.htmlEncode(Ext.getCmp("article_banner").getValue()),
                            article_content: Ext.htmlEncode(Ext.getCmp("kendoEditor").getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, "保存成功!");
                                editWin.close();
                                VoteArticle.load();
                            } else {
                                if (result.msg != null) {
                                    Ext.Msg.alert(INFORMATION, result.msg);
                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, FAILURE);
                                }
                            }
                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    })
                }
            }
        }]        
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: '文章詳情',
        id: 'editWin',
        iconCls: RowID ? "icon-user-edit" : "icon-user-add",
        width: 700,
        height: 500,
        layout: 'fit',
        items: [editFrm],
        constrain: true, //束縛窗口在框架內
        closeAction: 'destroy',
        modal: true,
        resizable: true,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: CLOSE,
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
                if (RowID) {
                    editFrm.getForm().loadRecord(RowID); //如果是編輯的話
                    initForm(RowID);
                }
                else {
                    editFrm.getForm().reset(); //如果不是編輯的話
                }
            }
        }
    });
    editWin.show();
    function initForm(Row) {
        var img = Row.data.article_banner.toString();
        var imgUrl = img.substring(img.lastIndexOf("\/") + 1);
        Ext.getCmp('article_banner').setRawValue(imgUrl);
        $('textarea[name=kendoEditor]').data("kendoEditor").value(Row.data.kendo_editor);
    }
}