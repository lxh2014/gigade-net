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
        url: '/News/NewsContentSave',
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
                id: 'news_id',
                name: 'news_id',
                hidden: true
            },
            {
                xtype: 'displayfield',
                fieldLabel: '上稿者',
                id: 'user_username',
                name: 'user_username',
                hidden: true
            },
            {
                xtype: 'textfield',
                fieldLabel: '標題',
                id: 'news_title',
                name: 'news_title',
                allowBlank: false
            },
            {
                xtype: 'numberfield',
                fieldLabel: '排序',
                id: 'news_sort',
                name: 'news_sort',
                minValue: 0,
                value: 0,
                allowBlank: false
            },
            {
                xtype: 'datetimefield',
                fieldLabel: '上線時間',
                id: 's_news_show_start',
                name: 's_news_show_start',
                format: 'Y-m-d H:i:s',
                editable: false,
                allowBlank: false,
                value: Tomorrow(),
                listeners: {
                    select: function () {
                        var Month = new Date(this.getValue()).getMonth() + 1;
                        Ext.getCmp("s_news_show_end").setValue(new Date(new Date(this.getValue()).setMonth(Month)));
                    }

                }
            },
            {
                xtype: 'datetimefield',
                fieldLabel: '下線時間',
                id: 's_news_show_end',
                name: 's_news_show_end',
                format: 'Y-m-d H:i:s',
                allowBlank: false,
                editable: false,
                value: new Date(Tomorrow().setMonth(Tomorrow().getMonth() + 1)),
                listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("s_news_show_start");
                        var end = Ext.getCmp("s_news_show_end");
                        var s_date = new Date(start.getValue());
                        if (end.getValue() < start.getValue()) {
                            Ext.Msg.alert("提示信息", "開始時間不能大於結束時間！");
                            end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                        }
                    }
                }


            },
            {
                xtype: 'textarea',
                fieldLabel: '最新消息內容',
                id: 'kendoEditor',
                name: 'kendoEditor'
            },
            {
                xtype: 'radiogroup',
                id: 'news_status',
                name: 'news_status',
                fieldLabel: '最新消息狀態',
                defaultType: 'radiofield',
                height: 50,
                hidden: true,
                defaults: {
                    flex: 1,
                    name: 'news_status',
                },
                columns: 3,
                vertical: true,
                items: [
                    {
                        boxLabel: '新建立',
                        id: 'new',
                        inputValue: 0,
                        checked: true
                    },
                      {
                          boxLabel: '顯示',
                          id: 'show',
                          inputValue: 1
                      },
                        {
                            boxLabel: '隱藏',
                            id: 'hide',
                            inputValue: 2
                        },
                          {
                              boxLabel: '下檔' + "(<span style='color:red'>小心！下檔後即無法再做任何變更！</span>)",
                              id: 'down',
                              inputValue: 3
                          }
                ]
            },
               {
                   xtype: 'displayfield',
                   fieldLabel: '建立時間',
                   id: 's_news_createdate',
                   name: 's_news_createdate',
                   hidden: true
               },
            {
                xtype: 'displayfield',
                fieldLabel: '更新時間',
                id: 's_news_updatedate',
                name: 's_news_updatedate',
                hidden: true
            },
            {
                xtype: 'displayfield',
                fieldLabel: '來源地址',
                id: 'news_ipfrom',
                name: 'news_ipfrom',
                hidden: true
            }
        ],
        buttons: [
            {
                formBind: true,
                disabled: true,
                text: '直接上稿',
                id: 'direct',
                handler: function () {
                    if (Ext.htmlEncode(Ext.getCmp('kendoEditor').getValue()) == "") {
                        Ext.Msg.alert("提示信息", '請輸入活動頁面內容');
                        return;
                    }
                    else {
                        var form = this.up('form').getForm();
                        if (form.isValid()) {
                            this.disable();
                            form.submit({
                                params: {
                                    news_id: Ext.htmlEncode(Ext.getCmp('news_id').getValue()),
                                    news_title: Ext.htmlEncode(Ext.getCmp('news_title').getValue()),
                                    news_sort: Ext.htmlEncode(Ext.getCmp('news_sort').getValue()),
                                    s_news_show_start: Ext.htmlEncode(Ext.getCmp('s_news_show_start').getValue()),
                                    s_news_show_end: Ext.htmlEncode(Ext.getCmp('s_news_show_end').getValue()),
                                    kendoEditor: Ext.htmlEncode(Ext.getCmp('kendoEditor').getValue()),
                                    news_status: Ext.htmlEncode(Ext.getCmp('news_status').getValue().news_status),
                                    type: 'direct'
                                    //,
                                    //s_news_createdate: Ext.htmlEncode(Ext.getCmp('s_news_createdate').getValue()),
                                    //s_news_updatedate: Ext.htmlEncode(Ext.getCmp('s_news_updatedate').getValue()),
                                    //news_ipfrom: Ext.htmlEncode(Ext.getCmp('news_ipfrom').getValue())
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(action.response.responseText);
                                    if (result.success) {
                                        Ext.Msg.alert(INFORMATION, "保存成功! ");
                                        store.load();
                                        editWin.close();
                                    }
                                    else {
                                        Ext.Msg.alert(INFORMATION, "保存失敗! ");
                                        EdmContentStore.load();
                                        editWin.close();
                                    }
                                }
                            });
                        }
                    }
                }

            },
            {
                formBind: true,
                disabled: true,
                text: '保存',
                handler: function () {
                    if (Ext.htmlEncode(Ext.getCmp('kendoEditor').getValue()) == "") {
                        Ext.Msg.alert("提示信息", '請輸入活動頁面內容');
                        return;
                    }
                    else {
                        var form = this.up('form').getForm();
                        if (form.isValid()) {
                            this.disable();
                            form.submit({
                                params: {
                                    news_id: Ext.htmlEncode(Ext.getCmp('news_id').getValue()),
                                    news_title: Ext.htmlEncode(Ext.getCmp('news_title').getValue()),
                                    news_sort: Ext.htmlEncode(Ext.getCmp('news_sort').getValue()),
                                    s_news_show_start: Ext.htmlEncode(Ext.getCmp('s_news_show_start').getValue()),
                                    s_news_show_end: Ext.htmlEncode(Ext.getCmp('s_news_show_end').getValue()),
                                    kendoEditor: Ext.htmlEncode(Ext.getCmp('kendoEditor').getValue())
                                    ,
                                    type: 'save',
                                    news_status: Ext.htmlEncode(Ext.getCmp('news_status').getValue().news_status)
                                    //,
                                    //s_news_createdate: Ext.htmlEncode(Ext.getCmp('s_news_createdate').getValue()),
                                    //s_news_updatedate: Ext.htmlEncode(Ext.getCmp('s_news_updatedate').getValue()),
                                    //news_ipfrom: Ext.htmlEncode(Ext.getCmp('news_ipfrom').getValue())
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(action.response.responseText);
                                    if (result.success) {
                                        Ext.Msg.alert(INFORMATION, "保存成功! ");
                                        store.load();
                                        editWin.close();
                                    }
                                    else {
                                        Ext.Msg.alert(INFORMATION, "保存失敗! ");
                                        EdmContentStore.load();
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
        title: '最新消息管理',
        iconCls: 'icon-user-edit',
        id: 'editWin',
        height: 335,
        width: 600,
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
            'beforerender': function () {
                if (row) {
                    Ext.getCmp('news_id').hidden = false;
                    Ext.getCmp('user_username').hidden = false;
                    Ext.getCmp('news_status').hidden = false;
                    Ext.getCmp('s_news_createdate').hidden = false;
                    Ext.getCmp('s_news_updatedate').hidden = false;
                    Ext.getCmp('news_ipfrom').hidden = false;
                    Ext.getCmp('direct').hidden = true;
                }
            },
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
        $('textarea[name=kendoEditor]').data("kendoEditor").value(row.data.news_content.replace(/>\s*<map/g, '><map'));
        switch (row.data.news_status) {
            case 0:
                Ext.getCmp('new').setValue(true);
                break;
            case 1:
                Ext.getCmp('show').setValue(true);
                break;
            case 2:
                Ext.getCmp('hide').setValue(true);
                break;
            case 3:
                Ext.getCmp('down').setValue(true);
                break;
        }
    }
}
function Today() {
    var d;
    var s = "";
    d = new Date();                             // 创建 Date 对象。
    s += d.getFullYear() + "/";                     // 获取年份。
    s += (d.getMonth() + 1) + "/";              // 获取月份。
    s += d.getDate();                          // 获取日。
    return (new Date(s));                                 // 返回日期。
};