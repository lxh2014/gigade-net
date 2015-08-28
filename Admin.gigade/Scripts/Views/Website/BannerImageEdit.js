editBannerImageFunction = function (row, store)
{
    var status = 0;
    if (row != null) {
        status = row.data.banner_status;
    }
    Ext.apply(Ext.form.VTypes, {
        daterange: function (val, field) {
            var date = field.parseDate(val);
            if (!date)
            {
                return;
            }
            if (field.startDateField && (!this.dateRangeMax || (date.getTime() != this.dateRangeMax.getTime())))
            {
                var start = Ext.getCmp(field.startDateField);
                start.setMaxValue(date);
                start.validate();
                this.dateRangeMax = date;
            } else if (field.endDateField && (!this.dateRangeMin || (date.getTime() != this.dateRangeMin.getTime())))
            {
                var end = Ext.getCmp(field.endDateField);
                end.setMinValue(date);
                end.validate();
                this.dateRangeMin = date;
            }
            return true;
        }
    });
    var editBannerImageFrm = Ext.create('Ext.form.Panel', {
        id: 'editBannerImageFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        url: '/Website/BannerImageEdit',
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 60 },
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: '圖片位置',
                id: 'site_name',
                name: 'site_name',
                value: document.getElementById('sname').value
            },
            {
                xtype: 'displayfield',
                fieldLabel: '流水號',
                id: 'banner_content_id',
                name: 'banner_content_id',
                hidden: row == null ? true : false,
                submitValue: true
            },
            {
                xtype: 'textfield',
                fieldLabel: '文字',
                id: 'banner_title',
                name: 'banner_title',
                submitValue: true
            },
            {
                xtype: 'textfield',
                fieldLabel: '連結',
                id: 'banner_link_url',
                name: 'banner_link_url',
                submitValue: true,
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
                             name: 'banner_link_mode',
                             id: 'oldw',
                             checked: true,
                             inputValue: 1
                         },
                         {
                             boxLabel: '新視窗開啟',
                             name: 'banner_link_mode',
                             id: 'neww',
                             inputValue: 2
                         }]
                     }

                 ]
             },
            {

                xtype: 'numberfield',
                fieldLabel: '排序',
                id: 'banner_sort',
                name: 'banner_sort',
                maxValue: 65536,
                minValue: 0
            },
             {
                 xtype: 'datefield',
                 fieldLabel: '上線日期',
                 id: 'banner_start',
                 name: 'banner_start',
                 allowBlank: false,
                 editable: false,
                 vtype: 'daterange',
                 endDateField: 'banner_end'
             },
              {
                  xtype: 'datefield',
                  fieldLabel: '下線日期',
                  id: 'banner_end',
                  name: 'banner_end',
                  allowBlank: false,
                  editable: false,
                  vtype: 'daterange',
                  startDateField: 'banner_start'
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
                         id: 'banner_statuses',
                         //margin: '0 0 0 10',
                         items: [{
                             boxLabel: '新建',
                             name: 'banner_status',
                             checked: true,
                             id: 's1',
                             inputValue: 0,
                             hidden:status== 0 ? false : true
                         },
                         {
                             boxLabel: '顯示',
                             name: 'banner_status',
                             id: 's2',
                             inputValue: 1
                         },
                         {
                             boxLabel: '隱藏',
                             name: 'banner_status',
                             id: 's3',
                             inputValue: 2
                         },
                         {
                             boxLabel: '下線/刪除',
                             name: 'banner_status',
                             id: 's4',
                             inputValue: 3
                         }]
                     }

                 ]
             },
             {
                 xtype: 'filefield',
                 name: 'banner_image',
                 id: 'banner_image',
                 fieldLabel: '圖檔',
                 msgTarget: 'side',
                 buttonText: '瀏覽..',
                 submitValue: true,
                 validator:
                 function (value)
                 {
                     var type = value.split('.');
                     if (type[type.length - 1] == 'gif' || type[type.length - 1] == 'png' || type[type.length - 1] == 'jpg')
                     {
                         return true;
                     }
                     else
                     {
                         return '上傳文件類型不正確！';
                     }
                 },
                 width: 300,
                 allowBlank: false,
                 fileUpload: true
             },
            {
                xtype: 'displayfield',
                fieldLabel: '建立時間',
                id: 'banner_createdate',
                name: 'banner_createdate',
                hidden: row == null ? true : false
            },
            {
                xtype: 'displayfield',
                fieldLabel: '修改時間',
                id: 'banner_updatedate',
                name: 'banner_updatedate',
                hidden: row == null ? true : false
            },
             {
                 xtype: 'displayfield',
                 fieldLabel: '來源IP',
                 id: 'banner_ipfrom',
                 name: 'banner_ipfrom',
                 hidden: row == null ? true : false
             }],
        buttons: [{
            formBind: true,
            disabled: true,
            text: SAVE,
            handler: function ()
            {
                var status = 0;
                if (Ext.getCmp('s1').getValue()) { status = 0; }
                if (Ext.getCmp('s2').getValue()) { status = 1; }
                if (Ext.getCmp('s3').getValue()) { status = 2; }
                if (Ext.getCmp('s4').getValue()) { status = 3; }
                var form = this.up('form').getForm();
                if (form.isValid())
                {
                    form.submit({
                        params: {
                            banner_content_id: Ext.htmlEncode(Ext.getCmp('banner_content_id').getValue()),
                            banner_site_id: document.getElementById('sid').value,
                            banner_title: Ext.htmlEncode(Ext.getCmp('banner_title').getValue()),
                            banner_link_url: Ext.htmlEncode(Ext.getCmp('banner_link_url').getValue()),
                            banner_sort: Ext.htmlEncode(Ext.getCmp('banner_sort').getValue()),
                            banner_start: Ext.htmlEncode(Ext.getCmp('banner_start').getValue()),
                            banner_end: Ext.htmlEncode(Ext.getCmp('banner_end').getValue()),
                            neww: Ext.htmlEncode(Ext.getCmp('neww').getValue()),
                            oldw: Ext.htmlEncode(Ext.getCmp('oldw').getValue()),
                            banner_statuses:status,
                            banner_image: Ext.htmlEncode(Ext.getCmp('banner_image').getValue())
                        },
                        success: function (form, action)
                        {
                            var result = Ext.JSON.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, result.msg);
                                store.load();
                                editBannerImageWin.close();
                            }                          
                        },
                        failure: function (form, action)
                        {
                            var result = Ext.JSON.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, result.msg);
                        }
                    });
                }
            }
        }]
    });
    var editBannerImageWin = Ext.create('Ext.window.Window', {
        iconCls: 'icon-user-edit',
        id: 'editBannerImageWin',
        width: 500,
        layout: 'fit',
        items: [editBannerImageFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
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
                         Ext.getCmp('editBannerImageWin').destroy();
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
                if (row != null) {
                    editBannerImageFrm.getForm().loadRecord(row);
                    Ext.getCmp('banner_start').setValue(row.data.banner_start.substring(0, 10));
                    Ext.getCmp('banner_end').setValue(row.data.banner_end.substring(0, 10));
                }
            }
        }
    });
    editBannerImageWin.show();
    initForm(row);
}
function initForm(row)
{
    var img = row.data.banner_image.toString();
    //var imgUrl = img.substring(img.lastIndexOf("\/") + 1);
    var url = img.split("\/");
    var imgUrl = url[url.length - 1];
    Ext.getCmp('banner_image').setRawValue(imgUrl);
}
