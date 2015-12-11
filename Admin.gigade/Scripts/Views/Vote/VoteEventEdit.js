editEventFunction = function (row, store)
{
    //实现验证开始时间必须小于结束时间
    Ext.apply(Ext.form.field.VTypes, {
        daterange: function (val, field)
        {
            var date = field.parseDate(val);
            if (!date)
            {
                return false;
            }
            this.dateRangeMax = null;
            this.dateRangeMin = null;
            if (field.startDateField && (!this.dateRangeMax || (date.getTime() != this.dateRangeMax.getTime())))
            {
                var start = field.up('form').down('#' + field.startDateField);
                start.setMaxValue(date);
                //start.validate();
                this.dateRangeMax = date;
            } else if (field.endDateField && (!this.dateRangeMin || (date.getTime() != this.dateRangeMin.getTime())))
            {
                var end = field.up('form').down('#' + field.endDateField);
                end.setMinValue(date);
                //end.validate();
                this.dateRangeMin = date;
            }
            return true;
        },
        daterangeText: '開始時間必須小於結束時間'
    });

    var editEventFrm = Ext.create('Ext.form.Panel', {
        id: 'editEventFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 80,
        url: '/Vote/SaveEvent',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: '活動編號',
                id: 'event_id',
                name: 'event_id',
                hidden: true,
                submitValue: true
            },
            {
                xtype: 'textfield',
                fieldLabel: '活動名稱',
                id: 'event_name',
                name: 'event_name',
                allowBlank: false,
                submitValue: true,
                listeners: {
                    blur: function () {
                        var name = Ext.getCmp('event_name').getValue();
                        if (name.length != 0) {
                            Ext.Ajax.request({
                                url: "/Vote/SelectByEventName",
                                method: 'post',
                                type: 'text',
                                params: {
                                    event_id: Ext.getCmp('event_id').getValue(),
                                    event_name: Ext.getCmp('event_name').getValue()
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(form.responseText);
                                    if (result.success) {
                                        msg = result.msg;
                                        if (msg == 1) {
                                            Ext.Msg.alert("提示", "活動標題已存在!");
                                            Ext.getCmp('event_name').setValue("");
                                            return false;
                                        }
                                    }
                                    else {
                                        Ext.Msg.alert("提示","查詢活動標題延遲!");
                                    }
                                }
                            });
                        }
                    }
                }
            },
            {
                xtype: 'textfield',
                fieldLabel: '活動描述',
                id: 'event_desc',
                name: 'event_desc',
                allowBlank: false,
                submitValue: true
            },
              {
                  fieldLabel: "開始時間",
                  xtype: "datetimefield",
                  id: 'event_start',
                  name: 'event_start',
                  allowBlank: false,
                  editable: false,
                  format: 'Y-m-d H:i:s',
                  value: Tomorrow(),
                  time: { hour: 00, min: 00, sec: 00 },
                  listeners: {
                      select: function (a, b, c) {
                          var start = Ext.getCmp("event_start");
                          var end = Ext.getCmp("event_end");
                          if (end.getValue() < start.getValue()) {
                              var start_date = start.getValue();
                              Ext.getCmp('event_end').setValue(new Date(start_date.getFullYear(), start_date.getMonth() + 1, start_date.getDate(), 23, 59, 59));
                          }
                      }
                  }
              },
              {
                  fieldLabel: "結束時間",
                  xtype: 'datetimefield',
                  id: 'event_end',
                  name: 'event_end',
                  format: 'Y-m-d H:i:s',
                  time: { hour: 23, min: 59, sec: 59 },
                  allowBlank: false,
                  editable: false,
                  value: setNextMonth(Tomorrow(), 1),
                  listeners: {
                      select: function (a, b, c) {
                          var start = Ext.getCmp("event_start");
                          var end = Ext.getCmp("event_end");
                          if (end.getValue() < start.getValue()) {//開始時間大於了結束時間
                              var end_date = end.getValue();
                              Ext.getCmp('event_start').setValue(new Date(end_date.getFullYear(), end_date.getMonth() - 1, end_date.getDate()));
                          }
                      }
                  }

              },
             {
                 xtype: 'filefield',
                 name: 'event_banner',
                 id: 'event_banner',
                 allowBlank: true,
                 fieldLabel: '活動圖檔',
                 msgTarget: 'side',
                 buttonText: '瀏覽..',
                 validator:
                 function (value) {
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
                 width: 300
             },
              {
                  xtype: 'numberfield',
                  fieldLabel: '字數長度',
                  id: 'word_length',
                  name: 'word_length',
                  minValue: 0,
                  value: 1,
                  submitValue: true
              },
              {
                  xtype: 'numberfield',
                  fieldLabel: '會員限制次數',
                  id: 'vote_everyone_limit',
                  name: 'vote_everyone_limit',
                  minValue: 0,
                  value: 1,
                  submitValue: true
              },
               {
                   xtype: 'numberfield',
                   fieldLabel: '每日限制次數',
                   id: 'vote_everyday_limit',
                   name: 'vote_everyday_limit',
                   minValue: 0,
                   value: 1,
                   submitValue: true
               },
               {
                   xtype: 'numberfield',
                   fieldLabel: '會員贈送次數',
                   id: 'number_limit',
                   name: 'number_limit',
                   minValue: 0,
                   value: 1,
                   submitValue: true
               },
               {
                   xtype: 'textfield',
                   fieldLabel: '促銷活動編號',
                   id: 'present_event_id',
                   name: 'present_event_id',
                   submitValue: true
               },
               {
                   xtype: 'fieldcontainer',
                   combineErrors: true,
                   layout: 'hbox',
                   width: 500,
                   items: [
                       {
                           xtype: 'displayfield',
                           value: '是否重複投票'
                       },
                       {
                           xtype: 'radiogroup',
                           allowBlank: false,
                           columns: 2,
                           id: 'is_repeat',
                           name: 'is_repeat',
                           width: 400,
                           margin: '0 0 0 25',
                           items: [{
                               boxLabel: '是',
                               name: 'is_repeat',
                               id:'y',
                               inputValue: 1
                           },
                           {
                               boxLabel: '否',
                               name: 'is_repeat',
                               inputValue: 0,
                               id: 'n',
                               checked: true
                           }]
                       }

                   ]
               },
               {
                   xtype: 'displayfield',
                   fieldLabel: '建立人',
                   id: 'cuser',
                   name: 'cuser',
                   hidden: row == null ? true : false
               },
               {
                   xtype: 'displayfield',
                   fieldLabel: '建立時間',
                   id: 'create_time',
                   name: 'create_time',
                   hidden: row == null ? true : false
               },
               {
                   xtype: 'displayfield',
                   fieldLabel: '修改人',
                   id: 'uuser',
                   name: 'uuser',
                   hidden: row == null ? true : false
               },
               {
                   xtype: 'displayfield',
                   fieldLabel: '修改時間',
                   id: 'update_time',
                   name: 'update_time',
                   hidden: row == null ? true : false
               }],
        buttons: [{
            formBind: true,
            disabled: true,
            text: SAVE,
            handler: function ()
            {
                var form = this.up('form').getForm();
                if (form.isValid())
                {
                    form.submit({
                        params: {
                            event_id: Ext.htmlEncode(Ext.getCmp('event_id').getValue()),
                            event_name: Ext.htmlEncode(Ext.getCmp('event_name').getValue()),
                            event_desc: Ext.htmlEncode(Ext.getCmp('event_desc').getValue()),
                            event_start: Ext.Date.format(new Date(Ext.getCmp('event_start').getValue()), 'Y-m-d H:i:s'),
                            event_end: Ext.Date.format(new Date(Ext.getCmp('event_end').getValue()), 'Y-m-d H:i:s'),
                            event_banner: Ext.htmlEncode(Ext.getCmp('event_banner').getValue()),
                            //banner_url: Ext.htmlEncode(Ext.getCmp('bannerUrl').getValue()),
                            word_length: Ext.htmlEncode(Ext.getCmp('word_length').getValue()),
                            vote_everyone_limit: Ext.htmlEncode(Ext.getCmp('vote_everyone_limit').getValue()),
                            vote_everyday_limit: Ext.htmlEncode(Ext.getCmp('vote_everyday_limit').getValue()),
                            number_limit: Ext.getCmp('number_limit').getValue(),
                            present_event_id: Ext.getCmp('present_event_id').getValue(),
                            is_repeat: Ext.getCmp('is_repeat').getValue()
                        },
                        success: function (form, action)
                        {
                            var result = Ext.JSON.decode(action.response.responseText);
                            if (result.success)
                            {
                                Ext.Msg.alert(INFORMATION, result.msg);
                                store.load();
                                editEventWin.close();

                            }

                        },
                        failure: function (form, action)
                        {
                            var result = Ext.JSON.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, result.msg);
                            store.load();
                            editEventWin.close();

                        }
                    });
                }
            }
        }]
    });
    var editEventWin = Ext.create('Ext.window.Window', {
        iconCls: 'icon-user-edit',
        id: 'editEventWin',
        width: 500,
        layout: 'fit',
        items: [editEventFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        title: row == null ? '活動新增' : '活動編輯',
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: '是否關閉',
             handler: function (event, toolEl, panel)
             {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn)
                 {
                     if (btn == "yes")
                     {
                         Ext.getCmp('editEventWin').destroy();
                     }
                     else
                     {
                         return false;
                     }
                 });
             }
         }
        ],
        listeners: {
            'show': function ()
            {
                if (row != null)
                {
                    editEventFrm.getForm().loadRecord(row);
                    if(row.data.is_repeat=="1")
                    {
                        Ext.getCmp("y").setValue(true);
                    }
                    if (row.data.is_repeat == "0")
                    {
                        Ext.getCmp("n").setValue(true);
                    }
                }

            }
        }
    });
    editEventWin.show();
    initForm(row);
}
function initForm(row)
{
    if (row != null)
    {
        var img = row.data.event_banner.toString();
        var imgUrl = img.substring(img.lastIndexOf("\/") + 1);
        Ext.getCmp('event_banner').setRawValue(imgUrl);
    }
}
function sformatDate(now)
{
    var year = now.getFullYear();
    var month = now.getMonth() + 1;
    var date = now.getDate();
    var hour = now.getHours();
    var minute = now.getMinutes();
    var second = now.getSeconds();
    return new Date(year + "-" + month + "-" + date + "   " + "00:00:00");
};
function eformatDate(now)
{
    now.setMonth(now.getMonth() + 1);
    return now;
};

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

  function setNextMonth (source, n) {
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
