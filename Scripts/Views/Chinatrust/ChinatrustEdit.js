editFunction = function (row, store) {
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

    var editUserFrm = Ext.create('Ext.form.Panel', {
        id: 'editUserFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        labelWidth: 40,
        url: '/Chinatrust/SaveChinatrust',
        defaults: { anchor: "95%", msgTarget: "side" },
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
                                "tooltip": "鏈接地址",
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
                                "tooltip": "上傳文件",
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
                                "tooltip": "查看html",
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
                fieldLabel: '編號',
                xtype: 'textfield',
                id: 'row_id',
                name: 'row_id',
                hidden: true
            },
            {
                fieldLabel: "活動名稱",
                xtype: 'textfield',
                padding: '0 0 0 0',
                id: 'event_name',
                name: 'event_name',
                allowBlank: false
            },
             {
                 xtype: 'textareafield',
                 fieldLabel: "活動描述",
                 id: 'kendoEditor',
                 name: 'kendoEditor'
             },
            {
                xtype: 'filefield',
                name: 'event_banner',
                id: 'event_banner',
                fieldLabel: '活動_banner',
                msgTarget: 'side',
                buttonText: '瀏覽..',
                validator:
               function (value) {
                   if (value != '') {
                       var type = value.split('.');
                       var extention = type[type.length - 1].toString().toLowerCase();
                       if (extention == 'gif' || extention == 'png' || extention == 'jpg') {
                           return true;
                       }
                       else {
                           return '上傳文件類型不正確！';
                       }
                   }
                   else {
                       return true;
                   }
               },
                submitValue: true,
                allowBlank: false,
                fileUpload: true
            },
            {
                fieldLabel: "會員註冊時間",
                xtype: 'datetimefield',
                id: 'user_register_time',
                name: 'user_register_time',
                allowBlank: false,
                editable: false,
                format: 'Y-m-d H:i:s',
                time: { hour: 00, min: 00, sec: 00 },
                value: Today()//new Date()
               
            },
             {
                 fieldLabel: "活動開始時間",
                 xtype: 'datetimefield',
                 id: 'event_start_time',
                 name: 'event_start_time',
                 allowBlank: false,
                 editable: false,
                 format: 'Y-m-d 00:00:00',
                 vtype: 'daterange',//標記類型
                 endDateField: 'event_end_time'//標記結束時間
             },
            {
                fieldLabel: "活動結束時間",
                xtype: 'datetimefield',
                id: 'event_end_time',
                name: 'event_end_time',
                format: 'Y-m-d 23:59:59',
                allowBlank: false,
                editable: false,
                vtype: 'daterange',
                startDateField: 'event_start_time'//標記開始時間
            }
        ],
        buttons: [{
            formBind: true,
            disabled: true,
            text: SAVE,
            handler: function () {
                var start = Ext.getCmp("event_start_time");
                var end = Ext.getCmp("event_end_time");
                if (end.getValue() < start.getValue()==true) {
                    Ext.Msg.alert("提示", "開始時間不能大於結束時間！");
                    return;
                }
                if (Ext.htmlEncode(Ext.getCmp("kendoEditor").getValue()).length > 1000==true) {
                     Ext.Msg.alert("提示", "元素內容已經超過1000，請修改您的元素內容");
                     return;
                 }
                var form = this.up('form').getForm();
                if (form.isValid()) {
                   
                    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: 'Loading...' });
                    myMask.show();
                    form.submit({
                        params: {
                            event_id: Ext.getCmp('row_id').getValue(),
                            event_name: Ext.htmlEncode(Ext.getCmp('event_name').getValue()),
                            event_desc: Ext.htmlEncode(Ext.getCmp("kendoEditor").getValue()),
                            event_start_time: Ext.getCmp('event_start_time').getValue(),
                            event_banner: Ext.htmlEncode(Ext.getCmp('event_banner').getValue()),
                            event_end_time: Ext.getCmp('event_end_time').getValue(),
                            user_register_time: Ext.getCmp('user_register_time').getValue()
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                myMask.hide();
                                if (result.msg != undefined) {
                                    Ext.Msg.alert("提示消息", result.msg);
                                } else {
                                    Ext.Msg.alert("提示消息", "保存成功");
                                    editUserWin.close();
                                    EventChinatrustStore.load();
                                }
                            }
                            else {
                                myMask.hide();
                                Ext.Msg.alert("提示消息", "操作失敗");
                                editUserWin.close();
                            }
                        },
                        failure: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            myMask.hide();
                            Ext.Msg.alert("提示信息", result.msg);
                        }
                    });
                }
            }
        }]
    });


    var editUserWin = Ext.create('Ext.window.Window', {
        id: 'editUserWin',
        width: 700,
        title: "中信活動",
        iconCls: 'icon-user-edit',
        iconCls: row ? "icon-user-edit" : "icon-user-add",
        layout: 'fit',
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        closable: false,
        items: [
                 editUserFrm
        ],
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        tools: [
         {
             type: 'close',
             qtip: "關閉",
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm("提示", "確定關閉?", function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('editUserWin').destroy();
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
                    editUserFrm.getForm().loadRecord(row); //如果是編輯的話
                    initForm(row);
                }
                else {
                    editUserFrm.getForm().reset(); //如果是新增的話
                }
            }
        }
    });
    if (row != null) {
        //if (!row.data.event_status) {
        //    Ext.getCmp('no').setValue(true);
        //}
        //else {
        //    Ext.getCmp('yes').setValue(true);
        //}
    }
    editUserWin.show();
    function initForm(Row) {
        var img = Row.data.event_banner.toString();
        var imgUrl = img.substring(img.lastIndexOf("\/") + 1);
        Ext.getCmp('event_banner').setRawValue(imgUrl);
        $('textarea[name=kendoEditor]').data("kendoEditor").value(Row.data.event_desc);
    }
}
