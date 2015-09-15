
editBannerNewsFunction = function (row, store)
{
    var status = 0;
    if (row != null) {
        status = row.data.news_status;
    }
    var history = document.getElementById('history').value;
    Ext.apply(Ext.form.field.VTypes, {
        //日期筛选
        daterange: function (val, field)
        {
            var date = field.parseDate(val);
            if (!date)
            {
                return false;
            }
            if (field.startDateField && (!this.dateRangeMax || (date.getTime() != this.dateRangeMax.getTime())))
            {
                var start = field.up('form').down('#' + field.startDateField);
                start.setMaxValue(date);
                start.validate();
                this.dateRangeMax = date;
            }
            else if (field.endDateField && (!this.dateRangeMin || (date.getTime() != this.dateRangeMin.getTime())))
            {
                var end = field.up('form').down('#' + field.endDateField);
                end.setMinValue(date);
                end.validate();
                this.dateRangeMin = date;
            }
            return true;
        },
        daterangeText: ''
    });
    function selectionChange()
    {
        var lis = $('.k-selectable').children();
        for (var i = 0; i < lis.length; i++)
        {
            var current = $(lis[i]);
            var selected = current.attr("aria-selected");
            if (selected == "true")
            {
                var dType = current.attr("data-type");
                if (dType == "d")
                {
                    url.val('http://');
                    return;
                }
                else if (dType == "f")
                {
                    url.val(decodeURIComponent(url.val()));
                    break;
                }
            }
        }
    }
    var editBannerNewsFrm = Ext.create('Ext.form.Panel', {
        id: 'editBannerNewsFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        autoScroll: true,
        url: '/Website/SaveBannerNewsContent',
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 90 },
        items: [
           {
               xtype: 'displayfield',
               fieldLabel: '文字廣告位置',
               value: document.getElementById('sname').value
           },
            {
                xtype: 'displayfield',
                fieldLabel: '文字廣告流水號',
                id: 'news_id',
                name: 'news_id',
                hidden: row == null ? true : false
            },
            {
                xtype: 'textfield',
                fieldLabel: '圖片位置',
                id: 'news_site_id',
                name: 'news_site_id',
                value: document.getElementById('sid').value,
                hidden: true
            },
            {
                xtype: 'textfield',
                fieldLabel: "文字標題",
                id: 'news_title',
                name: 'news_title',
                allowBlank: false,
                readOnly: history == 1 ? true : false
            },
            {
                xtype: 'textfield',
                fieldLabel: "文字廣告連結",
                id: 'news_link_url',
                name: 'news_link_url',
                vtype: 'url'
            },
             {
                 xtype: 'fieldcontainer',
                 combineErrors: true,
                 layout: 'hbox',
                 width: 500,
                 items: [
                     {
                         xtype: 'displayfield',
                         fieldLabel: '開啟模式'
                     },
                     {
                         xtype: 'radiogroup',
                         allowBlank: false,
                         columns: 2,
                         width: 300,
                         //margin: '0 0 0 10',
                         items: [{
                             boxLabel: '母視窗連接',
                             name: 'news_link_mode',
                             id:'m1',
                             checked: true,
                             inputValue: 1
                         },
                         {
                             boxLabel: '新視窗開啟',
                             name: 'news_link_mode',
                             id: 'm2',
                             inputValue: 2
                         }]
                     }

                 ]
             },
            {
                xtype: 'numberfield',
                fieldLabel: "排序顯示",
                id: 'news_sort',
                name: 'news_sort',
                value: 0,
                minValue: 0,
                maxValue: 65536
            },
            {
                fieldLabel: "上線日期",
                xtype: 'datefield',
                id: 'news_start',
                name: 'news_start',
                allowBlank: false,
                editable: false,
                format: 'Y-m-d',
                vtype: 'daterange',//標記類型
                endDateField: 'news_end'//標記結束時間
            },
            {
                fieldLabel: "下線日期",
                xtype: 'datefield',
                id: 'news_end',
                name: 'news_end',
                format: 'Y-m-d',
                allowBlank: false,
                editable: false,
                vtype: 'daterange',
                startDateField: 'news_start'//標記開始時間
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                     {
                         xtype: 'displayfield',
                         fieldLabel: '狀態'
                     },
                    {
                        xtype: 'radiogroup',
                        allowBlank: false,
                        width: 300,
                        columns: 4,
                        //margin: '0 0 0 10',
                        items: [{
                            boxLabel: '新建',
                            name: 'news_status',
                            checked: true,
                            id: 's1',
                            inputValue: 0,
                            hidden: status==0 ? false:true
                        },
                        {
                            boxLabel: '顯示',
                            name: 'news_status',
                            id: 's2',
                            inputValue: 1
                        },
                        {
                            boxLabel: '隱藏',
                            name: 'news_status',
                            id: 's3',
                            inputValue: 2
                        },
                        {
                            boxLabel: '下線/刪除',
                            name: 'news_status',
                            id: 's4',
                            inputValue: 3
                        }]
                    }

                ]
            },
            {
                xtype: 'displayfield',
                fieldLabel: "建立時間",
                id: 'news_createdate',
                name: 'news_createdate',
                hidden: row == null ? true : false
            },
             {
                 xtype: 'displayfield',
                 fieldLabel: "修改時間",
                 id: 'news_updatedate',
                 name: 'news_updatedate',
                 hidden: row == null ? true : false
             },
              {
                  xtype: 'displayfield',
                  fieldLabel: "來源IP",
                  id: 'news_ipfrom',
                  name: 'news_ipfrom',
                  hidden: row == null ? true : false
              }
        ],
        buttons: [
            {
                text: SAVE,
                formBind: true,
                disabled: true,
                hidden: history == 1 ? true : false,
                handler: function ()
                {
                    var form = this.up('form').getForm();
                    if (form.isValid())
                    {
                        var oldStatus = 0; //修改時原數據的狀態為不啟用，要修改為啟用時，並且當前啟用值大於等於限制值，並且值存在時才提示
                        if (row)
                        {
                            oldStatus = row.data.news_id;
                        }
                        form.submit({
                            params: {
                                news_id: Ext.getCmp("news_id").getValue(),
                                news_site_id: document.getElementById('sid').value,
                                news_title: Ext.htmlEncode(Ext.getCmp('news_title').getValue()),
                                news_link_url: Ext.htmlEncode(Ext.getCmp('news_link_url').getValue()),
                                news_sort: Ext.htmlEncode(Ext.getCmp('news_sort').getValue()),
                                news_start: Ext.htmlEncode(Ext.getCmp('news_start').getValue()),
                                news_end: Ext.htmlEncode(Ext.getCmp('news_end').getValue()),
                                //m1: Ext.htmlEncode(Ext.getCmp('m1').getValue()),
                                //m2: Ext.htmlEncode(Ext.getCmp('m2').getValue()),
                                //s1: Ext.htmlEncode(Ext.getCmp('s1').getValue()),
                                //s2: Ext.htmlEncode(Ext.getCmp('s2').getValue()),
                                //s3: Ext.htmlEncode(Ext.getCmp('s3').getValue()),
                                //s4: Ext.htmlEncode(Ext.getCmp('s4').getValue())
                            },
                            success: function (form, action)
                            {
                                var result = Ext.decode(action.response.responseText);
                                if (result.success)
                                {
                                    Ext.Msg.alert("提示信息", result.msg);
                                    editBannerNewsWin.close();
                                    BannerNewsContentStore.load();
                                } else
                                {
                                    Ext.Msg.alert("提示信息", result.msg);
                                }
                            },
                            failure: function ()
                            {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        })
                    }
                }
            }
        ]
    });
    var editBannerNewsWin = Ext.create('Ext.window.Window', {
        title: "廣告列表詳情",
        id: 'editBannerNewsWin',
        iconCls: row ? "icon-user-edit" : "icon-user-add",
        width: 500,
        layout: 'anchor',
        items: [editBannerNewsFrm],
        constrain: true, //束縛窗口在框架內
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
            {
                type: 'close',
                qtip: "關閉窗口",
                handler: function (event, toolEl, panel)
                {
                    Ext.MessageBox.confirm("確認", "是否確定關閉窗口?", function (btn)
                    {
                        if (btn == "yes")
                        {
                            Ext.getCmp('editBannerNewsWin').destroy();
                        } else
                        {
                            return false;
                        }
                    });
                }
            }
        ],
        listeners: {
            resize:function()
            {
                var h = Ext.getCmp('editBannerNewsFrm').getHeight() + 50;
                Ext.getCmp('editBannerNewsWin').setHeight(h);
                this.doLayout();
            },
            'show': function ()
            {
                if (row)
                {
                    editBannerNewsFrm.getForm().loadRecord(row); //如果是編輯的話
                    Ext.getCmp('news_start').setValue(row.data.news_start.substring(0, 10));
                    Ext.getCmp('news_end').setValue(row.data.news_end.substring(0, 10));
                    initForm(row);
                } else
                {
                    editBannerNewsFrm.getForm().reset(); //如果是編輯的話
                }
            }
        }
    });
    editBannerNewsWin.show();
    function initForm(row)
    {
        //var img = row.data.product_forbid_banner;
        //var imgUrl = img.substring(img.lastIndexOf("\/") + 1);
        //Ext.getCmp('product_forbid_banner').setRawValue(imgUrl);
        //img = row.data.product_active_banner;
        //imgUrl = img.substring(img.lastIndexOf("\/") + 1);
        //Ext.getCmp('product_active_banner').setRawValue(imgUrl);
        //Ext.getCmp('news_sort').hidden(false);
    }
}