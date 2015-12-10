//var subscriber = '<p style="text-align:center;"><span style="font-size:small;"><span style="color:#666666;"><a href="https://www.gigade100.com/member/mb_newsletter.php" target="_blank">訂閱/解訂電子報</a></span></span></p>';
var template_id_glo = 0;
var subscribe = document.getElementById('subscribe').value;
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
editFunction = function (row, store) {
    var split_str = document.getElementById('split_str').value;
    var template_data = "";
    var template = true;
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



    Ext.define('gigade.edm_pm', {
        extend: 'Ext.data.Model',
        fields: [
            { name: 'parameterCode', type: 'string' },
            { name: 'parameterName', type: 'string' }
        ]
    });
    var EdmPMStore = Ext.create("Ext.data.Store", {
        autoLoad: true,
        model: 'gigade.edm_pm',
        proxy: {
            type: 'ajax',
            url: '/EdmNew/GetEdmPMStore',
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
            { name: 'template_name', type: 'string' },
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


    EdmTemplateStore.on('beforeload', function () {
        Ext.apply(EdmTemplateStore.proxy.extraParams,
        {
            template_id: Ext.getCmp('template_id').getValue(),
        });
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
                value: 0,
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
                fieldLabel: '需求申請者',
                store: EdmPMStore,
                id: 'pm',
                name: 'pm',
                allowBlank: false,
                displayField: 'parameterName',
                valueField: 'parameterCode',
                editable: false,
                lastQuery: '',
            },
            {
                xtype: 'combobox',
                store: EdmTemplateStore,
                valueField: 'template_id',
                displayField: 'template_name',
                fieldLabel: '郵件範本',
                id: 'template_id',
                name: 'template_id',
                editable: false,
                lastQuery: '',
                editable: false,
                listeners: {
                    'select': function () {
                        var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                        var template_id_sel = Ext.getCmp('template_id').getValue();
                        var content_id_sel = Ext.getCmp('content_id').getValue();
                        Ext.getCmp('active').setValue();
                        Ext.getCmp('active_dis').setValue();
                        if (content_id_sel == "") {
                            NextAjax();
                        }
                        else {
                            //數據庫中有沒有 where  contnet_id=  and template_id=
                            //如果有就把html帶過來
                            //如果沒有就不帶過來
                            myMask.show();
                            Ext.Ajax.request({
                                url: '/EdmNew/GetHtml',
                                params: {
                                    content_id: Ext.getCmp('content_id').getValue(),
                                    template_id: Ext.getCmp('template_id').getValue(),
                                },
                                success: function (data) {
                                    myMask.hide();
                                    var result = data.responseText;
                                    if (result != "") {
                                        myMask.hide();
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
                                    else {
                                        NextAjax();
                                    }
                                },
                                failure: function () {
                                    myMask.hide();
                                    Ext.Msg.alert("提示信息", "出現異常");
                                }
                            });
                        }

                    }
                }
            },
                  {

                      xtype: 'fieldcontainer',
                      layout: 'hbox',
                      items: [
                      {
                          xtype: 'textfield',
                          name: 'active',
                          id: 'active',
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
                          xtype: 'displayfield',
                          id: 'active_dis',
                          name: 'active_dis',
                          submitValue:true,
                          hidden: true,
                          value:0,
                      },
                      {
                          xtype: 'button',
                          text: "載入",
                          id: 'btn_load',
                          margin: '0 0 0 5',
                          handler: LoadEpaperContent
                      },
                          {
                              xtype: 'checkbox',
                              id: 'check',
                              margin: '0 0 0 40',
                          },
                       {
                           xtype: 'displayfield',
                           margin: '0 0 0 5',
                           value: '是否添加訂閱',
                       }
                      ]

                  },
            {
                xtype: 'textarea',//520  740
                html: '<div id="template_area"></div>',
                frame: true,
                border: false
            },
                   {
                       xtype: 'fieldcontainer',
                       layout: 'hbox',
                       items: [
                       //{
                       //    xtype: 'checkbox',
                       //    id: 'check',
                       //    margin: '5 0 0 55',
                       //},
                       //{
                       //    xtype: 'displayfield',
                       //    margin: '5 0 0 5',
                       //    value: '是否添加訂閱',
                       //}
                       ]
                   },
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
                    try {
                        var editor1 = document.getElementById('editor').value;
                        var editor2 = document.getElementById('editor2').value;
                        template_data = editor1 + split_str + editor2;
                    } catch (e) {
                        template = false;
                    }
                    if (!template) {
                        try {
                            template_data = document.getElementById('editor3').value;
                        } catch (e) {
                            template_data = "";
                        }
                    }
                    if (form.isValid()) {
                        //this.disable();
                        form.submit({
                            params: {
                                content_id: Ext.htmlEncode(Ext.getCmp('content_id').getValue()),
                                sender_id: Ext.htmlEncode(Ext.getCmp('sender_id').getValue()),
                                group_id: Ext.htmlEncode(Ext.getCmp('group_id').getValue()),
                                importance: Ext.htmlEncode(Ext.getCmp('importance').getValue()),
                                subject: Ext.htmlEncode(Ext.getCmp('subject').getValue()),
                                template_id: Ext.htmlEncode(Ext.getCmp('template_id').getValue()),
                                active:Ext.htmlEncode(Ext.getCmp('active_dis').getValue()),
                                active_dis: Ext.htmlEncode(Ext.getCmp('active_dis').getValue()),
                                template_data: template_data,
                                pm: Ext.htmlEncode(Ext.getCmp('pm').getValue()),
                                check: Ext.getCmp('check').getValue(),
                             
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
            },
            {
                id: 'preview',
                text: '預覽',
                handler: function () {
                    PreviewFun(Ext.getCmp('content_id').getValue(), Ext.getCmp('check').getValue(),Ext.getCmp('template_id').getValue());
                }
            },
        ]
    });

    var editWin = Ext.create('Ext.window.Window', {
        title: '電子報新增/編輯',
        iconCls: 'icon-user-edit',
        id: 'editWin',
        height: 570,
        width: 793,
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
                    Ext.getCmp('preview').show(true);
                    initRow(row);
                }
                else {
                    EdmTemplateStore.on('load', function () {
                        Ext.getCmp('template_id').select(EdmTemplateStore.getAt(0));
                    });
                    Query(1);
                    Ext.getCmp('sender_id').allowBlank = false;
                    Ext.getCmp('group_id').allowBlank = false;
                    Ext.getCmp('template_id').allowBlank = false;
                    Ext.getCmp('preview').hide(true);
                    editFrm.getForm().reset();
                }
            }
        }
    });
    editWin.show();
    function initRow(row) {
        var result = row.data.template_data;
        if (result.indexOf(subscribe) > 0)
        {
            Ext.getCmp('check').setValue(true);
        }
        result = result.replace(subscribe, "");
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
        var url;
        if (x == 1) {
            url = "/EdmNew/Editkendo";
        }
        else {
            url = "/EdmNew/Editkendo2";
        }
        $.ajax({
            async: false,
            url: url,
        }).success(function (partialView) {
            $('#template_area').empty().append(partialView);
            // 呼叫 API 取得 edm_content.template_data
            $('.content_editor').kendoEditor({
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
                     //{
                     //    "name": "preview",
                     //    "tooltip": "預覽",
                     //    "exec":
                     //       function (e) {
                     //           var editor = $(this).data("kendoEditor");
                     //           var htmlsrc = Ext.htmlDecode(document.getElementById('editor3').value)  //$('.editor3]').data("kendoEditor");// Ext.getCmp('kendoEditor').getValue();
                     //           var A = 1000;
                     //           var B = 700;
                     //           var C = (document.body.clientWidth - A) / 2;
                     //           var D = window.open('', null, 'toolbar=yes,location=no,status=yes,menubar=yes,scrollbars=yes,resizable=yes,width=' + A + ',height=' + B + ',left=' + C);
                     //           var E = "<html><head><title>預覽</title></head><style>body{line-height:200%;padding:50px;}</style><body><div >" + htmlsrc + "</div></body></html>";
                     //           D.document.write(E);
                     //           D.document.close();
                     //       }
                     //},
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
    function NextAjax() {
        var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
        myMask.show();
        Ext.Ajax.request({
            url: '/EdmNew/GetEditUrlData',
            params: {
                template_id: Ext.getCmp('template_id').getValue(),
            },
            success: function (data) {
                myMask.hide();
                var result = data.responseText;
                if (result == "獲取網頁出現異常！") {
                    Ext.Msg.alert("提示信息", "獲取網頁出現異常！");
                }
                else {
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
            },
            failure: function () {
                myMask.hide();
                Ext.Msg.alert("提示信息", "獲取網頁出現異常！");
            }
        });
    }
     function LoadEpaperContent() {
        Ext.getCmp('check').setDisabled(false);
        Ext.getCmp('check').reset();
        var active = Ext.getCmp("active").getValue();
        if (active == null || active == "" || active == undefined) {
            Ext.Msg.alert("提示信息", "請輸入活動頁面id");
            return;
        }
        else {
            Ext.Ajax.request({
                url: '/Edm/LoadEpaperContent',
                method: 'post',
                params: {
                    content_id: active
                },
                success: function (form, action) {
                    var result = Ext.decode(form.responseText);
                    if (result.success) {
                        Query(1);
                        Ext.getCmp('active_dis').setValue(active);
                        $("#editor3").data("kendoEditor").value(Ext.htmlDecode(result.data.epaper_content).replace(/>\s*<map/g, '><map'));
                         
                    }
                    else {
                        if (result.msg == '0') {
                            Ext.Msg.alert("提示信息", '該活動頁面狀態為：新建，不可載入');
                        }
                        else if (result.msg == '1') {
                            Ext.Msg.alert("提示信息", '該活動頁面狀態為：隱藏，不可載入');
                        }
                        else if (result.msg == '2') {
                            Ext.Msg.alert("提示信息", '該活動頁面狀態為：下檔，不可載入');
                        }
                        else if (result.msg == '3') {
                            Ext.Msg.alert("提示信息", '該活動頁面不存在');
                        }
                        else {
                            Ext.Msg.alert("提示信息", "失敗！");
                        }
                    }
                },
                failure: function (form, action) {
                    var result = Ext.decode(form.responseText);
                    if (result.msg == '0') {
                        Ext.Msg.alert("提示信息", '該活動頁面狀態為：新建，不可載入');
                    }
                    else if (result.msg == '1') {
                        Ext.Msg.alert("提示信息", '該活動頁面狀態為：隱藏，不可載入');
                    }
                    else if (result.msg == '2') {
                        Ext.Msg.alert("提示信息", '該活動頁面狀態為：下檔，不可載入');
                    }
                    else if (result.msg == '3') {
                        Ext.Msg.alert("提示信息", '該活動頁面不存在');
                    }
                    else {
                        Ext.Msg.alert("提示信息", "失敗！");
                    }
                }
            })
        }
     }
    
}