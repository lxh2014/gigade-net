var SizeStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
             { "txt": "725px", "value": "725px" },
             { "txt": "900px", "value": "900px" }
    ]
});

var TypeStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
             { "txt": "吉刻美食", "value": "0" },
             { "txt": "電子報", "value": "1" },
             { "txt": "中信獨家熱門", "value": "2" },
             { "txt": "頁嵌夾報", "value": "3" },
               { "txt": "臺新活動情報", "value": "4" },
                 { "txt": "健康2.0", "value": "5" }
    ]
});


function HtmlChange() {
    if ($('.k-editor-textarea').val().indexOf('<map') >= 0) {
        var b = $('.k-editor-textarea').val().replace(/>\s*<map/g, '><map');
        $('.k-editor-textarea').val(b);
    }
}



Ext.onReady(function () {


    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/EpaperContent/SaveEpaperContent',
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
                var row = document.getElementById("epaperId").value;
                if (row != "") {
                    Ext.Ajax.request({
                        url: '/EpaperContent/GetEpaperById',
                        method: 'post',
                        async: false,
                        reader: {
                            type: 'json',
                            root: 'data'
                        },
                        params: {
                            rowid: row
                        },
                        success: function (form, action) {
                            var result = Ext.decode(form.responseText);
                            if (result.success) {
                                Ext.getCmp("epaper_id").setValue(result.data.epaper_id);
                                Ext.getCmp("user_username").setValue(result.data.user_username);
                                Ext.getCmp("epaper_title").setValue(result.data.epaper_title);
                                Ext.getCmp("epaper_short_title").setValue(result.data.epaper_short_title);
                                Ext.getCmp("epaper_size").setValue(result.data.epaper_size);
                                Ext.getCmp("type").setValue(result.data.type);
                                Ext.getCmp("epaper_sort").setValue(result.data.epaper_sort);
                                Ext.getCmp("epaperShowStart").setValue(result.data.epaperShowStart);
                                Ext.getCmp("epaperShowEnd").setValue(result.data.epaperShowEnd);
                                Ext.getCmp("epaper_status").setValue(result.data.epaper_status);
                                Ext.getCmp("epaperCreateDate").setValue(result.data.epaperCreateDate);
                                Ext.getCmp("epaperUpdateDate").setValue(result.data.epaperUpdateDate);
                                Ext.getCmp("epaper_ipfrom").setValue(result.data.epaper_ipfrom);
                                Ext.getCmp("fb_description").setValue(result.data.fb_description);
                                $('textarea[name=kendoEditor]').data("kendoEditor").value(result.data.epaper_content);
                                if (result.data.epaper_status == 0) {
                                    Ext.getCmp('new').setValue(true);
                                }
                                else if (result.data.epaper_status == 1) {
                                    Ext.getCmp('show').setValue(true);
                                }
                                else if (result.data.epaper_status == 2) {
                                    Ext.getCmp('hide').setValue(true);
                                }
                                else {
                                    Ext.getCmp('down').setValue(true);
                                }
                            }

                        }
                    });
                }
            },
            'beforerender': function () {
                var row = document.getElementById("epaperId").value;
                if (row != "") {
                    Ext.getCmp('epaper_status').hidden = false;
                    Ext.getCmp('epaperCreateDate').hidden = false;
                    Ext.getCmp('epaperUpdateDate').hidden = false;
                    Ext.getCmp('epaper_ipfrom').hidden = false;
                    Ext.getCmp('fb_description').hidden = false;
                    Ext.getCmp('direct').hidden = true;
                    Ext.getCmp('FBDispaly').hidden = false;
                }
            }
        },
        /////編輯器
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: '編號',
                id: 'epaper_id',
                name: 'epaper_id',
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
                id: 'epaper_title',
                name: 'epaper_title',
                allowBlank: false
            },
            {
                xtype: 'textfield',
                fieldLabel: '短標題',
                id: 'epaper_short_title',
                name: 'epaper_short_title'
            },
            {
                xtype: 'combobox',
                store: SizeStore,
                id: 'epaper_size',
                name: 'epaper_size',
                fieldLabel: '尺寸',
                displayField: 'txt',
                valueField: 'value',
                value: '725px',
                editable: false
            },
             {
                 xtype: 'combobox',
                 store: TypeStore,
                 fieldLabel: '類型',
                 id: 'type',
                 name: 'type',
                 displayField: 'txt',
                 valueField: 'value',
                 value: '0',
                 editable: false
             },
            {
                xtype: 'numberfield',
                fieldLabel: '排序',
                id: 'epaper_sort',
                name: 'epaper_sort',
                minValue: 0,
                value: 0,
                allowDecimals: false,
                allowBlank: false
            },
            {
                xtype: 'datetimefield',
                fieldLabel: '上線時間',
                id: 'epaperShowStart',
                name: 'epaperShowStart',
                format: 'Y-m-d H:i:s',
                allowBlank: false,
                editable: false,
                value: Tomorrow(),
                listeners: {
                    select: function () {
                        var Month = new Date(this.getValue()).getMonth() + 1;
                        Ext.getCmp("epaperShowEnd").setValue(new Date(new Date(this.getValue()).setMonth(Month)));
                    }
                }
            },
            {
                xtype: 'datetimefield',
                fieldLabel: '下線時間',
                id: 'epaperShowEnd',
                name: 'epaperShowEnd',
                format: 'Y-m-d H:i:s',
                editable: false,
                allowBlank: false,
                value: new Date(Tomorrow().setMonth(Tomorrow().getMonth() + 1)),
                listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("epaperShowStart");
                        var end = Ext.getCmp("epaperShowEnd");
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
                id: 'epaper_status',
                name: 'epaper_status',
                fieldLabel: '活動頁面狀態',
                defaultType: 'radiofield',
                hidden: true,
                defaults: {
                    flex: 1,
                    name: 'epaper_status',
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
                id: 'epaperCreateDate',
                name: 'epaperCreateDate',
                hidden: true
            },
            {
                xtype: 'displayfield',
                fieldLabel: '更新時間',
                id: 'epaperUpdateDate',
                name: 'epaperUpdateDate',
                hidden: true
            },
            {
                xtype: 'displayfield',
                fieldLabel: '來源地址',
                id: 'epaper_ipfrom',
                name: 'epaper_ipfrom',
                hidden: true
            },
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                    {
                        xtype: 'textfield',
                        fieldLabel: '簡述(FB Share)',
                        id: 'fb_description',
                        name: 'fb_description',
                        hidden: true
                    },
                    {
                        xtype: 'displayfield',
                        id: 'FBDispaly',
                        name: 'FBDispaly',
                        value: '(要使用FB Share功能，在圖片Tag裡面加上 onclick="fbs_click();return false;" 即可)',
                        hidden: true
                    }
                ]
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
                                    epaper_id: Ext.htmlEncode(Ext.getCmp('epaper_id').getValue()),
                                    epaper_title: Ext.htmlEncode(Ext.getCmp('epaper_title').getValue()),
                                    epaper_short_title: Ext.htmlEncode(Ext.getCmp('epaper_short_title').getValue()),
                                    epaper_size: Ext.htmlEncode(Ext.getCmp('epaper_size').getValue()),
                                    type: Ext.htmlEncode(Ext.getCmp('type').getValue()),
                                    epaper_sort: Ext.htmlEncode(Ext.getCmp('epaper_sort').getValue()),
                                    epaperShowStart: Ext.htmlEncode(Ext.getCmp('epaperShowStart').getValue()),
                                    epaperShowEnd: Ext.htmlEncode(Ext.getCmp('epaperShowEnd').getValue()),
                                    kendoEditor: Ext.htmlEncode(Ext.getCmp('kendoEditor').getValue()),
                                    epaper_status: Ext.htmlEncode(Ext.getCmp('epaper_status').getValue().epaper_status),
                                    newsType: 'direct'
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(action.response.responseText);
                                    if (result.success) {
                                        if (Ext.getCmp('epaper_id').getValue() == "") {
                                            Ext.Msg.confirm("提示信息", "保存成功,是否關閉?", function (btn) {
                                                if (btn == "yes") {
                                                    //window.parent.Ext.getCmp('ContentPanel').activeTab.close();
                                                    var panel = window.parent.Ext.getCmp('ContentPanel');
                                                    var actab = panel.activeTab;
                                                    var t = panel.getActiveTab().prev();
                                                    if (t) {
                                                        panel.setActiveTab(t);
                                                        actab.close();
                                                        panel.doLayout();
                                                    }
                                                }
                                                else {
                                                    Ext.getCmp('editFrm').getForm().reset();
                                                    $('textarea[name=kendoEditor]').data("kendoEditor").value('');
                                                }
                                            });
                                        }

                                    }
                                    else {
                                        Ext.Msg.alert(INFORMATION, "保存失敗! ");
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
                                    epaper_id: Ext.htmlEncode(Ext.getCmp('epaper_id').getValue()),
                                    epaper_title: Ext.htmlEncode(Ext.getCmp('epaper_title').getValue()),
                                    epaper_short_title: Ext.htmlEncode(Ext.getCmp('epaper_short_title').getValue()),
                                    epaper_size: Ext.htmlEncode(Ext.getCmp('epaper_size').getValue()),
                                    type: Ext.htmlEncode(Ext.getCmp('type').getValue()),
                                    epaper_sort: Ext.htmlEncode(Ext.getCmp('epaper_sort').getValue()),
                                    epaperShowStart: Ext.htmlEncode(Ext.getCmp('epaperShowStart').getValue()),
                                    epaperShowEnd: Ext.htmlEncode(Ext.getCmp('epaperShowEnd').getValue()),
                                    kendoEditor: Ext.htmlEncode(Ext.getCmp('kendoEditor').getValue()),
                                    epaper_status: Ext.htmlEncode(Ext.getCmp('epaper_status').getValue().epaper_status),
                                    fb_description: Ext.htmlEncode(Ext.getCmp('fb_description').getValue()),
                                    newsType: 'save'
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(action.response.responseText);
                                    if (result.success) {
                                        if (Ext.getCmp('epaper_id').getValue() == "") {
                                            Ext.Msg.confirm("提示信息", "保存成功,是否關閉?", function (btn) {
                                                if (btn == "yes") {
                                                    var panel = window.parent.Ext.getCmp('ContentPanel');
                                                    var actab = panel.activeTab;
                                                    var t = panel.getActiveTab().prev();
                                                    if (t) {
                                                        panel.setActiveTab(t);
                                                        actab.close();
                                                        panel.doLayout();
                                                    }
                                                    //window.parent.Ext.getCmp('ContentPanel').activeTab.close();
                                                }
                                                else {
                                                    Ext.getCmp('editFrm').getForm().reset();
                                                    $('textarea[name=kendoEditor]').data("kendoEditor").value('');
                                                }
                                            });
                                        }
                                        else {
                                            var panel = window.parent.Ext.getCmp('ContentPanel');
                                            var actab = panel.activeTab;
                                            var t = panel.getActiveTab().prev();
                                            if (t) {
                                                panel.setActiveTab(t);
                                                actab.close();
                                                panel.doLayout();
                                            }
                                            //window.parent.Ext.getCmp('ContentPanel').activeTab.close();
                                        }
                                    }
                                    else {
                                        Ext.Msg.alert(INFORMATION, "保存失敗! ");
                                    }
                                }
                            });
                        }
                    }
                }
            }
        ]
    });


    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [editFrm],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                editFrm.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });

    function initRow(row) {
        $('textarea[name=kendoEditor]').data("kendoEditor").value(row.data.epaper_content);
        switch (row.data.epaper_status) {
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
});

function Today() {

    //時間
    var d;
    d = new Date();                             // 创建 Date 对象。
    d.setDate(d.getDate());
    return d;


};


//時間

function Tomorrow() {
    var d;
    d = new Date();                             // 创建 Date 对象。
    d.setDate(d.getDate() + 1);
    return d;
}
