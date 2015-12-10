Ext.apply(Ext.form.field.VTypes, {
    //日期筛选
    daterange: function (val, field) {
        var date = field.parseDate(val);
        if (!date) {
            return false;
        }
        if (field.startDateField && (!this.dateRangeMax || (date.getTime() != this.dateRangeMax.getTime()))) {
            var start = field.up('form').down('#' + field.startDateField);
            start.setMaxValue(date);
            start.validate();
            this.dateRangeMax = date;
        }
        else if (field.endDateField && (!this.dateRangeMin || (date.getTime() != this.dateRangeMin.getTime()))) {
            var end = field.up('form').down('#' + field.endDateField);
            end.setMinValue(date);
            end.validate();
            this.dateRangeMin = date;
        }
        return true;
    },
    daterangeText: ''
});
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
                fieldLabel: "活動名稱",
                allowBlank: false,
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
                labelWidth: 110,
                emptyText: '請選擇',
                listeners: {
                    select: function () {
                        Ext.Ajax.request({
                            url: 'SelMaxSort',
                            params: {
                                event_id: Ext.getCmp('event_id').getValue()
                            },
                            success: function (form, action) {
                                var result = Ext.decode(form.responseText);
                                if (result.success) {
                                    Ext.getCmp('article_sort').setValue(result.sort);
                                }
                            }
                        });
                    }
                },
            },
            {
                xtype: 'textfield',
                fieldLabel: "商品編號",
                labelWidth: 110,
                id: 'product_id',
                name: 'product_id',
                listeners: {
                    blur: function () {
                        var id = Ext.getCmp('product_id').getValue();
                        Ext.Ajax.request({
                            url: "/Vote/GetProductName",
                            method: 'post',
                            type: 'text',
                            params: {
                                id: Ext.getCmp('product_id').getValue()
                            },
                            success: function (form, action) {
                                var result = Ext.decode(form.responseText);
                                if (result.success) {
                                    msg = result.msg;
                                    if (msg == 0) {
                                        Ext.getCmp("product_name").setValue("沒有該商品信息！");
                                        Ext.getCmp("product_id").setValue("");
                                    }
                                    else {
                                        Ext.getCmp("product_name").setValue(msg);
                                    }
                                }
                                else {
                                    Ext.getCmp("product_name").setValue("沒有該商品信息！");
                                    Ext.getCmp("product_id").setValue("");
                                }
                            }
                        });
                    }
                }
            },
            {
                xtype: 'displayfield',
                fieldLabel: "品名",
                labelWidth: 110,
                name: 'product_name',
                id: 'product_name'
            },
            {
                xtype: 'textfield',
                fieldLabel: "商品鏈接",
                labelWidth: 110,
                id: 'prod_link',
                name: 'prod_link',
                allowBlank: false,
                vtype:'url',
            },
            {
                xtype: 'textfield',
                fieldLabel: "會員編號",
                labelWidth: 110,
                id: 'user_id',
                name: 'user_id',
                listeners: {
                    blur: function () {
                        var id = Ext.getCmp('product_id').getValue();
                        Ext.Ajax.request({
                            url: "/Vote/GetUserName",
                            method: 'post',
                            type: 'text',
                            params: {
                                id: Ext.getCmp('user_id').getValue()
                            },
                            success: function (form, action) {
                                var result = Ext.decode(form.responseText);
                                if (result.success) {
                                    msg = result.msg;
                                    if (msg == 0) {
                                        Ext.getCmp("name").setValue("沒有該會員編號信息！");
                                        Ext.getCmp("user_id").setValue("");
                                    }
                                    else {
                                        Ext.getCmp("name").setValue(msg);
                                    }
                                }
                                else {
                                    Ext.getCmp("name").setValue("沒有該會員編號信息！");
                                    Ext.getCmp("user_id").setValue("");
                                }
                            }
                        });
                    }
                }
            },
            {
                xtype: 'displayfield',
                fieldLabel: "會員名稱",
                labelWidth: 110,
                name: 'name',
                id: 'name'
            },
            {
                xtype: 'textfield',
                allowBlank: false,
                fieldLabel: "文章標題",
                labelWidth: 110,
                id: 'article_title',
                name: 'article_title',
                listeners: {
                    blur: function () {
                        var name = Ext.getCmp('article_title').getValue();
                        if (name.length != 0) {
                            Ext.Ajax.request({
                                url: "/Vote/SelectByArticleName",
                                method: 'post',
                                type: 'text',
                                params: {
                                    id:ID,
                                    article_title: Ext.getCmp('article_title').getValue()
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(form.responseText);
                                    if (result.success) {
                                        msg = result.msg;
                                        if (msg == 1) {
                                            Ext.Msg.alert("提示","文章標題已存在!");
                                            Ext.getCmp('article_title').setValue("");
                                            return false;
                                        }
                                    }
                                    else {
                                        Ext.Msg.alert("提示","查詢文章標題延遲!");
                                    }
                                }
                            });
                        }
                    }
                }
            },
            {//專區Banner                
                xtype: 'filefield',
                name: 'article_banner',
                id: 'article_banner',
                fieldLabel: "文章大圖",
                msgTarget: 'side',
                buttonText: '選擇...',
                validator:
                function (value)
                {
                    if (value != '')
                    {
                        var type = value.split('.');
                        var extention = type[type.length - 1].toString().toLowerCase();
                        if (extention == 'gif' || extention == 'png' || extention == 'jpg')
                        {
                            return true;
                        }
                        else
                        {
                            return '上傳文件類型不正確！';
                        }
                    }
                    else
                    {
                        return true;
                    }
                },
                submitValue: true,
                labelWidth: 110
            },
            {
                xtype: 'numberfield',
                fieldLabel: "排序",
                minValue: 0,
                maxValue:999999,
                labelWidth: 110,
                id: 'article_sort',
                name: 'article_sort',
                allowDecimals: false,
                value: 0,
            },
            {
                xtype: 'numberfield',
                fieldLabel: "前台顯示投票數量",
                labelWidth: 110,
                id: 'vote_count',
                name: 'vote_count',
                maxValue: 999999,
                hideTrigger: true,
                mouseWheelEnabled: true,
                allowDecimals:false,
                allowNegative: false,
                minValue:0,
                value: 0

            
     
      
       
            },
            {
                xtype: 'textareafield',
                fieldLabel: "文章內容",
                labelWidth: 110,
                id: 'kendoEditor',
                name: 'kendoEditor'
            },
            {
                xtype: 'datetimefield',
                fieldLabel: '文章開始時間',
                id: 'article_start_time',
                name: 'article_start_time',
                labelWidth: 110,
                format: 'Y-m-d H:i:s',
                editable: false,
                allowBlank: false,
                value:Tomorrow(),
                time: { hour: 00, min: 00, sec: 00 },
                listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("article_start_time");
                        var end = Ext.getCmp("article_end_time");
                        if (end.getValue() < start.getValue()) {
                            var start_date = start.getValue();
                            Ext.getCmp('article_end_time').setValue(new Date(start_date.getFullYear(), start_date.getMonth() + 1, start_date.getDate(), 23, 59, 59));
                        }
                    }
                }
            },
           {
                   xtype: 'datetimefield',
                   fieldLabel: '文章結束時間',
                    id: 'article_end_time',
                    name: 'article_end_time',
                    labelWidth: 110,
                    format: 'Y-m-d H:i:s',
                    editable: false,
                    allowBlank: false,
                    time: { hour: 23, min: 59, sec: 59 },
                    value: setNextMonth(Tomorrow(), 1),
                    listeners: {
                        select: function (a, b, c) {
                            var start = Ext.getCmp("article_start_time");
                            var end = Ext.getCmp("article_end_time");
                            if (end.getValue() < start.getValue()) {//開始時間大於了結束時間
                                var end_date = end.getValue();
                                Ext.getCmp('article_start_time').setValue(new Date(end_date.getFullYear(), end_date.getMonth() - 1, end_date.getDate()));
                            }
                        }
                    }
           },
           {
               xtype: 'datetimefield',
               fieldLabel: '顯示開始時間',
               id: 'article_show_start_time',
               name: 'article_show_start_time',
               labelWidth: 110,
               time: { hour: 00, min: 00, sec: 00 },
               format: 'Y-m-d H:i:s',
               editable: false,
               allowBlank: false,
               value: Tomorrow(),
               listeners: {
                   select: function (a, b, c) {
                       var start = Ext.getCmp("article_show_start_time");
                       var end = Ext.getCmp("article_show_end_time");
                       if (end.getValue() < start.getValue()) {
                           var start_date = start.getValue();
                           Ext.getCmp('article_show_end_time').setValue(new Date(start_date.getFullYear(), start_date.getMonth() + 1, start_date.getDate(), 23, 59, 59));
                       }
                   }
               }
           },
           {
               xtype: 'datetimefield',
               fieldLabel: '顯示結束時間',
               id: 'article_show_end_time',
               name: 'article_show_end_time',
               labelWidth: 110,
               format: 'Y-m-d H:i:s',
               allowBlank: false,
               editable: false,
               time: { hour: 23, min: 59, sec: 59 },
               value: setNextMonth(Tomorrow(), 1),
               listeners: {
                   select: function (a, b, c) {
                       var start = Ext.getCmp("article_show_start_time");
                       var end = Ext.getCmp("article_show_end_time");
                       if (end.getValue() < start.getValue()) {//開始時間大於了結束時間
                           var end_date = end.getValue();
                           Ext.getCmp('article_show_start_time').setValue(new Date(end_date.getFullYear(), end_date.getMonth() - 1, end_date.getDate()));
                       }
                   }
               }
           },
        ],
        buttons: [
        {
            text: '保存',
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (Ext.htmlEncode(Ext.getCmp("kendoEditor").getValue()).length > 5000) {
                    Ext.Msg.alert(INFORMATION, ELECONTENTTIP);
                    return;
                }
                if (Ext.getCmp('article_start_time').getValue() > Ext.getCmp('article_end_time').getValue()) {
                    Ext.Msg.alert("提示信息", "文章開始時間不能大於文章結束時間");
                    return;
                }
                if (Ext.getCmp('article_show_start_time').getValue() > Ext.getCmp('article_show_end_time').getValue()) {
                    Ext.Msg.alert("提示信息", "顯示開始時間不能大於顯示結束時間");
                    return;
                }
                if (form.isValid()) {
                    //if (Ext.htmlEncode(Ext.getCmp("kendoEditor").getValue()) == "") {
                    //    Ext.Msg.alert(INFORMATION, KENDOTIP);
                    //    return;
                    //}
                
                    form.submit({
                        params: {
                            id: ID,
                            event_id: Ext.htmlEncode(Ext.getCmp("event_id").getValue()),
                            prod_link: Ext.htmlEncode(Ext.getCmp("prod_link").getValue()),
                            user_id: Ext.htmlEncode(Ext.getCmp("user_id").getValue()),
                            article_title: Ext.htmlEncode(Ext.getCmp("article_title").getValue()),
                            article_sort: Ext.htmlEncode(Ext.getCmp("article_sort").getValue()),
                            vote_count: Ext.htmlEncode(Ext.getCmp("vote_count").getValue()),
                            article_banner: Ext.htmlEncode(Ext.getCmp("article_banner").getValue()),
                            article_content: Ext.htmlEncode(Ext.getCmp("kendoEditor").getValue()),
                            article_start_time: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('article_start_time').getValue()), 'Y-m-d H:i:s')),
                            article_end_time: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('article_end_time').getValue()), 'Y-m-d H:i:s')),
                            article_show_start_time: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('article_show_start_time').getValue()), 'Y-m-d H:i:s')),
                            article_show_end_time: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('article_show_end_time').getValue()), 'Y-m-d H:i:s')),
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
        width: 760,
        height: 530,
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
function Tomorrow() {
    var d;
    var dt;
    var s = "";
    d = new Date();                             // 创建 Date 对象。
    s += d.getFullYear() + "/";                     // 获取年份。
    s += (d.getMonth() + 1) + "/";              // 获取月份。
    s += d.getDate();
    dt = new Date(s);
    dt.setDate(dt.getDate() + 1);
    return dt;                                 // 返回日期。
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