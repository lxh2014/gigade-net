var number = 0;
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
function HtmlChange() {
    if ($('.k-editor-textarea').val().indexOf('<map') >= 0) {
        var b = $('.k-editor-textarea').val().replace(/>\s*<map/g, '><map');
        $('.k-editor-textarea').val(b);
    }
}
Ext.onReady(function () {
    //alert(document.getElementById("EdmStoreHidden").value);
    Ext.define('gigade.EpaperContent', {
        extend: 'Ext.data.Model',
        fields: [
            { name: "epaper_id", type: "int" },
             { name: "epaper_title", type: "string" },
             { name: "epaper_content", type: "string" },
        ]
    });
    var EpaperContentSendStore = Ext.create('Ext.data.Store', {
        autoLoad: true,
        model: 'gigade.EpaperContent',
        proxy: {
            type: 'ajax',
            url: '/Edm/GetEpaperContent',
            reader: {
                type: 'json',
                root: 'data'
            }
        }
        //    autoLoad: true
    });

    Ext.define('gigade.EdmContent', {
        extend: 'Ext.data.Model',
        fields: [
            { name: "group_name", type: "string" },
             { name: "group_id", type: "int" }
        ]
    });

    var EdmGroupSendStore = Ext.create('Ext.data.Store', {
        autoLoad: true,
        model: 'gigade.EdmContent',
        proxy: {
            type: 'ajax',
            url: '/Edm/GetEdmGroup',
            reader: {
                type: 'json',
                root: 'data'
            }
        }
        //    autoLoad: true
    });
    var EdmDisStore = Ext.create('Ext.data.Store', {
        fields: ['txt', 'value'],
        data: [
            { "txt": '一般', "value": "1" },
            { "txt": '重要', "value": "2" },
            { "txt": '不重要', "value": "3" }
        ]
    });
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/Edm/EdmContentSave',
        defaults: { anchor: '95%', msgTarget: "side" },
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
                                        $('.k-window').css('height', 340);
                                        $('.k-window').css('width', 700);
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
                var row = document.getElementById("EdmStoreHidden").value;
                Ext.Ajax.request({//根據area_id查詢出該區域的element_type
                    url: '/Edm/GetEpaperContentById',
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
                            if (result.data.content_status == 5) {
                                Ext.getCmp("save").hide(true);
                            }
                            Ext.getCmp("content_id").setValue(result.data.content_id);
                            Ext.getCmp("startdate").setValue(result.data.s_content_start);
                            Ext.getCmp("content_from_name").setValue(result.data.content_from_name);
                            Ext.getCmp("content_from_email").setValue(result.data.content_from_email);
                            Ext.getCmp("content_reply_email").setValue(result.data.content_reply_email);
                            Ext.getCmp("group_id").setValue(result.data.group_id);
                            Ext.getCmp("editve").setValue(result.data.info_epaper_id);
                            Ext.getCmp("edm_dis").setValue(result.data.content_priority);
                            Ext.getCmp("content_title").setValue(result.data.content_title);
                            $('textarea[name=kendoEditor]').data("kendoEditor").value(result.data.content_body);
                            if (result.data.content_body.indexOf('訂閱/解訂電子報') >= 0) {
                                number = -1;
                                Ext.getCmp('check').setValue(true);
                            }
                        }

                    }
                });
            }
        },
        /////編輯器
        items:
            [
            {
                xtype: 'textfield',
                fieldLabel: '編號',
                id: 'content_id',
                name: 'content_id',
                submitValue: true,
                hidden: true
            },
        {
            xtype: "datetimefield",
            fieldLabel: '發送時間',
            editable: false,
            id: 'startdate',
            name: 'start_date',
            format: 'Y-m-d H:i:s',
            // minValue: new Date(),
            time: { hour: 00, min: 00, sec: 00 },//開始時間00:00:00
            allowBlank: false,
            submitValue: true,
            value: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate() + 1, 00, 00, 00)
        },
        {
            xtype: 'textfield',
            name: 'content_from_name',
            id: 'content_from_name',
            submitValue: true,
            fieldLabel: '寄件者名稱',
            allowBlank: false,
            value: '吉甲地市集電子報'
        },
        {
            xtype: 'textfield',
            name: 'content_from_email',
            id: 'content_from_email',
            submitValue: true,
            fieldLabel: '寄件者郵箱',
            allowBlank: false,
            value: 'gigade100@gigade100.com',
            vtype: 'email'
        },
        {
            xtype: 'textfield',
            name: 'content_reply_email',
            id: 'content_reply_email',
            submitValue: true,
            fieldLabel: '回覆郵箱',
            allowBlank: false,
            value: 'gigade100@gigade100.com',
            vtype: 'email'
        },
        {
            xtype: 'combobox',
            name: 'group_id',
            id: 'group_id',
            editable: false,
            fieldLabel: '收件者名單',
            store: EdmGroupSendStore,
            // queryMode: 'local',
            submitValue: true,
            displayField: 'group_name',
            valueField: 'group_id',
            typeAhead: true,
            forceSelection: false,
            lastQuery: '',
            value: '1'
        },
        {
            xtype: 'combobox',
            name: 'edm_dis',
            id: 'edm_dis',
            editable: false,
            fieldLabel: '郵件重要度',
            store: EdmDisStore,
            // queryMode: 'local',
            submitValue: true,
            displayField: 'txt',
            valueField: 'value',
            typeAhead: true,
            forceSelection: false,
            lastQuery: '',
            value: 1
        },
        {
            xtype: 'textfield',
            name: 'content_title',
            id: 'content_title',
            allowBlank: false,
            submitValue: true,
            fieldLabel: '郵件主旨'
        },
       {

           xtype: 'fieldcontainer',
           layout: 'hbox',
           items: [
               {
                   xtype: 'textfield',
                   name: 'editve',
                   id: 'editve',
                   submitValue: true,
                   fieldLabel: '活動頁面',
                   emptyText: '請輸入活動頁面id',
                   listeners: {
                       specialkey: function (field, e) {
                           if (e.getKey() == e.ENTER) {
                               LoadEpaperContent();
                           }
                       }
                   }
               },
               {
                   xtype: 'button',
                   text: "載入",
                   id: 'btn_load',
                   margin: '0 0 0 5',
                   handler: LoadEpaperContent
               }
           ]

       },
        {
            xtype: 'textareafield',
            name: 'kendoEditor',
            id: 'kendoEditor',
            fieldLabel: '郵件內容',
            allowBlank: true,
            submitValue: true
        },
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                {
                    xtype: 'checkbox',
                    id: 'check',
                    margin: '5 0 0 55',
                    handler: checked

                },
                {
                    xtype: 'displayfield',
                    margin: '5 0 0 5',
                    value: '是否添加訂閱',
                }
                ]
            }
            ],
        buttons: [{
            formBind: true,
            disabled: true,
            id: 'save',
            text: '保存',
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    var startdate = formatDate(Ext.getCmp('startdate').getValue());
                    if (Ext.getCmp('kendoEditor').getValue() == "") {
                        Ext.Msg.alert(INFORMATION, EDMEMAILTIP);
                        return;
                    }
                    this.disable();
                    form.submit({
                        params: {
                            content_id: Ext.htmlEncode(Ext.getCmp('content_id').getValue()),
                            startdate: startdate,
                            content_from_name: Ext.htmlEncode(Ext.getCmp('content_from_name').getValue()),
                            content_from_email: Ext.htmlEncode(Ext.getCmp('content_from_email').getValue()),
                            content_reply_email: Ext.htmlEncode(Ext.getCmp('content_reply_email').getValue()),
                            group_id: Ext.htmlEncode(Ext.getCmp('group_id').getValue()),
                            epaper_id: Ext.htmlEncode(Ext.getCmp('editve').getValue()),
                            edm_dis: Ext.htmlEncode(Ext.getCmp('edm_dis').getValue()),
                            content_title: Ext.htmlEncode(Ext.getCmp('content_title').getValue()),
                            kendoEditor: Ext.getCmp('kendoEditor').getValue()
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                if (Ext.getCmp('content_id').getValue() == "") {
                                    Ext.MessageBox.confirm('提示信息', '保存成功,是否關閉？', function (btn) {
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
                                    Ext.Msg.alert("提示信息", "保存成功");
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



                            }
                            else {
                                //Ext.MessageBox.confirm('提示信息', '保存失敗,是否關閉？', function (btn) {
                                //    if (btn == "yes") {
                                //        window.parent.Ext.getCmp('ContentPanel').activeTab.close();
                                //    }
                                //    //else {
                                //    //    Ext.getCmp('editFrm').getForm().reset();
                                //    //    $('textarea[name=kendoEditor]').data("kendoEditor").value('');
                                //    //}
                                //});
                                ExtMsg.alert("提示信息", "保存失敗!");
                            }
                        },
                        failure: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.MessageBox.confirm('Confirm', '保存失敗,是否關閉？', function (btn) {
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
                    });
                }
            }
        }]
    });
    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [editFrm],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                editFrm.width = document.documentElement.clientWidth - 10;
                editFrm.height = document.documentElement.clientHeight - 10;
                this.doLayout();
            }
        }
    });
    function formatDate(now) {
        var year = now.getFullYear();
        var month = now.getMonth() + 1;
        var date = now.getDate();
        var hour = now.getHours();
        var minute = now.getMinutes();
        var second = now.getSeconds();
        return year + "-" + month + "-" + date + "   " + hour + ":" + minute + ":" + second;
    };
    function getDate() {
        var now = new Date();
        var year = now.getFullYear();
        var month = now.getMonth() + 1;
        var date = now.getDate();
        var hour = now.getHours();
        var minute = now.getMinutes();
        var second = now.getSeconds();
        return year + "-" + month + "-" + date + "   " + hour + ":" + minute + ":" + second;
    };
});
var LoadEpaperContent = function () {
    number = 0;
    Ext.getCmp('check').setDisabled(false);
    Ext.getCmp('check').reset();
    var editve = Ext.getCmp("editve").getValue();
    if (editve == null || editve == "" || editve == undefined) {
        Ext.Msg.alert(INFORMATION, "請輸入活動頁面id");
    }
    else {
        Ext.Ajax.request({
            url: '/Edm/LoadEpaperContent',
            method: 'post',
            params: {
                content_id: editve
            },
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result.success) {
                    var epaperContent = result.data.epaper_content;
                    $('textarea[name=kendoEditor]').data("kendoEditor").value(Ext.util.Format.htmlDecode(epaperContent));
                }
                else {
                    if (result.msg == '0') {
                        Ext.Msg.alert(INFORMATION, '該活動頁面狀態為：新建，不可載入');
                    }
                    else if (result.msg == '1') {
                        Ext.Msg.alert(INFORMATION, '該活動頁面狀態為：隱藏，不可載入');
                    }
                    else if (result.msg == '2') {
                        Ext.Msg.alert(INFORMATION, '該活動頁面狀態為：下檔，不可載入');
                    }
                    else if (result.msg == '3') {
                        Ext.Msg.alert(INFORMATION, '該活動頁面不存在');
                    }
                    else {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                }
            },
            failure: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result.msg == '0') {
                    Ext.Msg.alert(INFORMATION, '該活動頁面狀態為：新建，不可載入');
                }
                else if (result.msg == '1') {
                    Ext.Msg.alert(INFORMATION, '該活動頁面狀態為：隱藏，不可載入');
                }
                else if (result.msg == '2') {
                    Ext.Msg.alert(INFORMATION, '該活動頁面狀態為：下檔，不可載入');
                }
                else if (result.msg == '3') {
                    Ext.Msg.alert(INFORMATION, '該活動頁面不存在');
                }
                else {
                    Ext.Msg.alert(INFORMATION, FAILURE);
                }
            }
        })
    }
}
var checked = function () {
    if (Ext.getCmp('check').checked) {
        number += 1;
        var text = Ext.util.Format.htmlDecode(Ext.getCmp('kendoEditor').getValue());
        text += "<p style=" + "text-align:center;" + "><font size=" + "2" + "><font color=" + "#666666" + "><a href=" +
            "https://www.gigade100.com/member/mb_newsletter.php" + " target=" + "_blank" + ">訂閱/解訂電子報</a></font></font></p><p> &nbsp;</p><p> &nbsp;</p>";
        Ext.getCmp('check').setDisabled(true);
        if (number != 0) {
            $('textarea[name=kendoEditor]').data("kendoEditor").value(text);
        }
    }
}