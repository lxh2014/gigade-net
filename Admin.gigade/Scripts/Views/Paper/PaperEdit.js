editPaperFunction = function (row, store) {
    //实现验证开始时间必须小于结束时间
    Ext.apply(Ext.form.VTypes, {
        daterange: function (val, field) {
            var date = field.parseDate(val);
            if (!date) {
                return;
            }
            if (field.startDateField
                    && (!this.dateRangeMax || (date.getTime() != this.dateRangeMax
                            .getTime()))) {
                var start = Ext.getCmp(field.startDateField);
                start.setMaxValue(date);
                start.validate();
                this.dateRangeMax = date;
            } else if (field.endDateField
                    && (!this.dateRangeMin || (date.getTime() != this.dateRangeMin
                            .getTime()))) {
                var end = Ext.getCmp(field.endDateField);
                end.setMinValue(date);
                end.validate();
                this.dateRangeMin = date;
            }
            /*
             * Always return true since we're only using this vtype to set
             * the min/max allowed values (these are tested for after the
             * vtype test)
             */
            return true;
        }
    });

    var editPaperFrm = Ext.create('Ext.form.Panel', {
        id: 'editPaperFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 80,
        url: '/Paper/PaperEdit',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: '流水號',
                id: 'paperID',
                name: 'paperID',
                hidden: true,
                submitValue: true
            },
            {
                xtype: 'textfield',
                fieldLabel: '問卷名稱',
                id: 'paperName',
                name: 'paperName',
                allowBlank: false,
                submitValue: true
            },
            {
                xtype: 'textfield',
                fieldLabel: '問卷備註',
                id: 'paperMemo',
                name: 'paperMemo',
                submitValue: true
            },
              {
                  xtype: "datefield",
                  fieldLabel: '開始時間',
                  id: 'paperStart',
                  name: 'paperStart',
                  format: 'Y-m-d',
                  allowBlank: false,
                  submitValue: true,
                  value: new Date(),
                  editable:false,
                  //vtype: 'daterange',//daterange类型为上代码定义的类型
                  //endDateField: 'paperEnd'//必须跟endDate的id名相同
                  //value: sformatDate(new Date()),
                  listeners: {
                      'select': function () {
                          var start = Ext.getCmp('paperStart').getValue();
                          Ext.getCmp('paperEnd').setMinValue(start);
                          var endDate = Ext.getCmp('paperEnd').getValue();
                          if (start > endDate) {
                              var stime = new Date(start);
                              stime.setMonth(stime.getMonth() + 1);
                              Ext.getCmp('paperEnd').setValue(stime);
                          }
                      }
                  }
              },
               {
                   xtype: "datefield",
                   fieldLabel: '結束時間',
                   id: 'paperEnd',
                   name: 'paperEnd',
                   format: 'Y-m-d',
                   editable: false,
                   allowBlank: false,
                   submitValue: true,
                   value: eformatDate(new Date()),
                   //vtype: 'daterange',//daterange类型为上代码定义的类型
                   //startDateField: 'paperStart'//必须跟endDate的id名相同
                   listeners: {
                       select: function () {
                           var start = Ext.getCmp('paperStart').getValue();
                           var endDate = Ext.getCmp('paperEnd').getValue();
                           if (start > endDate) {
                               //Ext.getCmp('paperStart').setValue(endDate);
                               var etime = new Date(endDate);
                               etime.setMonth(etime.getMonth() - 1);
                               Ext.getCmp('paperStart').setValue(etime);
                           }
                       }
                   }

               },            
             {
                 xtype: 'filefield',
                 name: 'paperBanner',
                 id: 'paperBanner',
                 allowBlank: true,
                 fieldLabel: '問卷圖檔',
                 msgTarget: 'side',
                 buttonText: '瀏覽..',
                 //validator:
                 //function (value) {
                 //    if (value != '') {
                 //        var type = value.split('.');
                 //        if (type[type.length - 1] == 'gif' || type[type.length - 1] == 'png' || type[type.length - 1] == 'jpg') {
                 //            return true;
                 //        }
                 //        else {
                 //            return '上傳文件類型不正確！';
                 //        }
                 //    }
                 //},
                 submitValue: true,
                 width: 300
             },
            {
                xtype: 'textfield',
                fieldLabel: '廣告鏈接',
                id: 'bannerUrl',
                name: 'bannerUrl',
                vtype: 'url',
                submitValue: true
            },
             //{
             //    xtype: 'fieldcontainer',
             //    combineErrors: true,
             //    layout: 'hbox',
             //    //defaults: {
             //    //    flex: 0.5
             //    //},
             //    width: 500,
             //    items: [
             //        {
             //            xtype: 'displayfield',
             //            value: '是否關聯促銷'
             //        },
             //        {
             //            xtype: 'radiogroup',
             //            allowBlank: false,
             //            columns: 2,
             //            width: 400,
             //            margin: '0 0 0 25',
             //            items: [{
             //                boxLabel: '是',
             //                name: 'isPromotion',
             //                id: 'ypromo',
             //                inputValue: 1,
             //                listeners: {
             //                    change: function (radio, newValue, oldValue) {
             //                        var promotionurl = Ext.getCmp("promotionUrl");
             //                        if (newValue) {
             //                            promotionurl.setValue('');
             //                            promotionurl.setDisabled(false);
             //                        }
             //                    }
             //                }
             //            },
             //            {
             //                boxLabel: '否',
             //                name: 'isPromotion',
             //                id: 'npromo',
             //                checked: true,
             //                inputValue: 0,
             //                listeners: {
             //                    change: function (radio, newValue, oldValue) {
             //                        var promotionurl = Ext.getCmp("promotionUrl");
             //                        if (newValue) {
             //                            promotionurl.setValue('');
             //                            promotionurl.setDisabled(true);
             //                        }
             //                    }
             //                }

             //            }]
             //        }

             //    ]
             //},
             //{
             //    xtype: 'textfield',
             //    fieldLabel: '促銷鏈接',
             //    id: 'promotionUrl',
             //    name: 'promotionUrl',
             //    vtype: 'url',
             //    disabled: true,
             //    submitValue: true
             //},
             // {
             //     xtype: 'fieldcontainer',
             //     combineErrors: true,
             //     layout: 'hbox',
             //     //defaults: {
             //     //    flex: 0.5
             //     //},
             //     width: 500,
             //     items: [
             //         {
             //             xtype: 'displayfield',
             //             value: '是否贈送購物金'
             //         },
             //         {
             //             xtype: 'radiogroup',
             //             allowBlank: false,
             //             columns: 2,
             //             width: 400,
             //             margin: '0 0 0 25',
             //             items: [{
             //                 boxLabel: '是',
             //                 name: 'isGiveBonus',
             //                 id: 'ygb',
             //                 inputValue: 1,
             //                 listeners: {
             //                     change: function (radio, newValue, oldValue) {
             //                         var bonusnum = Ext.getCmp("bonusNum");
             //                         if (newValue) {
             //                             bonusnum.setValue(0);
             //                             bonusnum.setDisabled(false);
             //                         }
             //                     }
             //                 }
             //             },
             //             {
             //                 boxLabel: '否',
             //                 name: 'isGiveBonus',
             //                 id: 'ngb',
             //                 inputValue: 0,
             //                 checked: true,
             //                 listeners: {
             //                     change: function (radio, newValue, oldValue) {
             //                         var bonusnum = Ext.getCmp("bonusNum");
             //                         if (newValue) {
             //                             bonusnum.setValue(0);
             //                             bonusnum.setDisabled(true);
             //                         }
             //                     }
             //                 }
             //             }]
             //         }

             //     ]
             // },
             //{
             //    xtype: 'numberfield',
             //    fieldLabel: '購物金金額',
             //    id: 'bonusNum',
             //    name: 'bonusNum',
             //    disabled: true,
             //    minValue: 0,
             //    submitValue: true
             //},
             // {
             //     xtype: 'fieldcontainer',
             //     combineErrors: true,
             //     layout: 'hbox',
             //     //defaults: {
             //     //    flex: 0.5
             //     //},
             //     width: 500,
             //     items: [
             //         {
             //             xtype: 'displayfield',
             //             value: '是否贈送贈品'
             //         },
             //         {
             //             xtype: 'radiogroup',
             //             allowBlank: false,
             //             columns: 2,
             //             width: 400,
             //             margin: '0 0 0 25',
             //             items: [{
             //                 boxLabel: '是',
             //                 name: 'isGiveProduct',
             //                 id: 'ygp',
             //                 inputValue: 1,
             //                 listeners: {
             //                     change: function (radio, newValue, oldValue) {
             //                         var productid = Ext.getCmp("productID");
             //                         if (newValue) {
             //                             productid.setValue('');
             //                             productid.setDisabled(false);
             //                         }
             //                     }
             //                 }
             //             },
             //             {
             //                 boxLabel: '否',
             //                 name: 'isGiveProduct',
             //                 id: 'ngp',
             //                 inputValue: 0,
             //                 checked: true,
             //                 listeners: {
             //                     change: function (radio, newValue, oldValue) {
             //                         var productid = Ext.getCmp("productID");
             //                         if (newValue) {
             //                             productid.setValue('');
             //                             productid.setDisabled(true);
             //                         }
             //                     }
             //                 }
             //             }]
             //         }

             //     ]
             // },
             //{
             //    xtype: 'textfield',
             //    fieldLabel: '贈送商品編號',
             //    id: 'productID',
             //    name: 'productID',
             //    disabled: true,
             //    submitValue: true,
             //    listeners: {
             //        blur: function () {
             //            var id = Ext.getCmp('productID').getValue();
             //            Ext.Ajax.request({
             //                url: "/WareHouse/GetProdInfo",
             //                method: 'post',
             //                type: 'text',
             //                params: {
             //                    id: id
             //                },
             //                success: function (form, action) {
             //                    var result = Ext.decode(form.responseText);
             //                    if (result.success) {
             //                        Ext.getCmp('product_name').setValue(result.msg);
             //                        Ext.getCmp('product_name').show();
             //                    }
             //                    else {
             //                        Ext.getCmp("product_name").setValue("沒有該商品信息！");
             //                        Ext.getCmp('product_name').show();

             //                    }
             //                }
             //            });
             //        }
             //    }
             //},
             // {
             //     xtype: 'displayfield',
             //     fieldLabel: '商品名稱',
             //     hidden: true,
             //     id: 'product_name',
             //     name: 'product_name'
             // },
              {
                  xtype: 'textfield',
                  fieldLabel: '贈送活動ID',
                  id: 'event_ID',
                  name: 'event_ID',
                  maxLength: 10,
                  submitValue: true
              },
              {
                  xtype: 'fieldcontainer',
                  combineErrors: true,
                  layout: 'hbox',
                  //defaults: {
                  //    flex: 0.5
                  //},
                  width: 500,
                  items: [
                      {
                          xtype: 'displayfield',
                          value: '是否重複贈送'
                      },
                      {
                          xtype: 'radiogroup',
                          allowBlank: false,
                          columns: 2,
                          width: 400,
                          margin: '0 0 0 25',
                          items: [{
                              boxLabel: '是',
                              name: 'isRepeatGift',
                              id: 'yrg',
                              inputValue: 1
                          },
                          {
                              boxLabel: '否',
                              name: 'isRepeatGift',
                              id: 'nrg',
                              inputValue: 0,
                              checked: true
                          }]
                      }

                  ]
              },
              {
                  xtype: 'fieldcontainer',
                  combineErrors: true,
                  layout: 'hbox',
                  //defaults: {
                  //    flex: 0.5
                  //},
                  width: 500,
                  items: [
                      {
                          xtype: 'displayfield',
                          value: '是否重複填寫'
                      },
                      {
                          xtype: 'radiogroup',
                          allowBlank: false,
                          columns: 2,
                          width: 400,
                          margin: '0 0 0 25',
                          items: [{
                              boxLabel: '是',
                              name: 'isRepeatWrite',
                              id: 'yrw',
                              inputValue: 1
                          },
                          {
                              boxLabel: '否',
                              name: 'isRepeatWrite',
                              id: 'nrw',
                              checked: true,
                              inputValue: 0
                          }]
                      }

                  ]
              },
              {
                  xtype: 'fieldcontainer',
                  combineErrors: true,
                  layout: 'hbox',
                  //defaults: {
                  //    flex: 0.5
                  //},
                  width: 500,
                  items: [
                      {
                          xtype: 'displayfield',
                          value: '是否為新會員填寫'
                      },
                      {
                          xtype: 'radiogroup',
                          allowBlank: false,
                          columns: 2,
                          width: 400,
                          margin: '0 0 0 14',
                          items: [{
                              boxLabel: '是',
                              name: 'isNewMember',
                              id: 'ynm',
                              inputValue: 1
                          },
                          {
                              boxLabel: '否',
                              name: 'isNewMember',
                              id: 'nnm',
                              checked: true,
                              inputValue: 0
                          }]
                      }

                  ]
              },
            {
                xtype: 'displayfield',
                fieldLabel: '建立時間',
                id: 'created',
                name: 'created',
                hidden: true
            },
            {
                xtype: 'displayfield',
                fieldLabel: '修改時間',
                id: 'modified',
                name: 'modified',
                hidden: true
            },
             {
                 xtype: 'displayfield',
                 fieldLabel: '來源IP',
                 id: 'ipfrom',
                 name: 'ipfrom',
                 hidden: true
             }],
        buttons: [{
            formBind: true,
            disabled: true,
            text: SAVE,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            paper_id: Ext.htmlEncode(Ext.getCmp('paperID').getValue()),
                            paper_name: Ext.htmlEncode(Ext.getCmp('paperName').getValue()),
                            paper_memo: Ext.htmlEncode(Ext.getCmp('paperMemo').getValue()),
                            paper_start: Ext.Date.format(new Date(Ext.getCmp('paperStart').getValue()), 'Y-m-d H:i:s'),
                            paper_end: Ext.Date.format(new Date(Ext.getCmp('paperEnd').getValue()), 'Y-m-d H:i:s'),
                            paper_banner: Ext.htmlEncode(Ext.getCmp('paperBanner').getValue()),
                            banner_url: Ext.htmlEncode(Ext.getCmp('bannerUrl').getValue()),
                            //ypromo: Ext.htmlEncode(Ext.getCmp('ypromo').getValue()),
                            //npromo: Ext.htmlEncode(Ext.getCmp('npromo').getValue()),
                            //promotion_url: Ext.htmlEncode(Ext.getCmp('promotionUrl').getValue()),
                            //ygb: Ext.htmlEncode(Ext.getCmp('ygb').getValue()),
                            //ngb: Ext.htmlEncode(Ext.getCmp('ngb').getValue()),
                            //bonus_num: Ext.htmlEncode(Ext.getCmp('bonusNum').getValue()),
                            //ygp: Ext.htmlEncode(Ext.getCmp('ygp').getValue()),
                            //ngp: Ext.htmlEncode(Ext.getCmp('ngp').getValue()),
                            //product_id: Ext.htmlEncode(Ext.getCmp('productID').getValue()),
                            eventid: Ext.getCmp('event_ID').getValue(),
                            yrg: Ext.htmlEncode(Ext.getCmp('yrg').getValue()),
                            nrg: Ext.htmlEncode(Ext.getCmp('nrg').getValue()),
                            yrw: Ext.htmlEncode(Ext.getCmp('yrw').getValue()),
                            nrw: Ext.htmlEncode(Ext.getCmp('nrw').getValue()),
                            ynm: Ext.htmlEncode(Ext.getCmp('ynm').getValue()),
                            nnm: Ext.htmlEncode(Ext.getCmp('nnm').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.JSON.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, result.msg);
                                store.load();
                                editPaperWin.close();

                            }

                        },
                        failure: function (form, action) {
                            var result = Ext.JSON.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, result.msg);
                            store.load();
                            editPaperWin.close();

                        }
                    });
                }
            }
        }]
    });
    var editPaperWin = Ext.create('Ext.window.Window', {
        iconCls: 'icon-user-edit',
        id: 'editPaperWin',
        width: 500,
        layout: 'fit',
        items: [editPaperFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        title: row == null ? '問卷新增' : '問卷編輯',
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
                         Ext.getCmp('editPaperWin').destroy();
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
                if (row != null) {
                    editPaperFrm.getForm().loadRecord(row);
                    if (row.data.paperStart != "") {
                        Ext.getCmp('paperStart').setValue(row.data.paperStart.substring(0,10));
                    }
                    if (row.data.paperStart != "") {
                        Ext.getCmp('paperEnd').setValue(row.data.paperEnd.substring(0, 10));
                    }
                                    
                    Ext.getCmp('created').show();
                    Ext.getCmp('modified').show();
                    Ext.getCmp('ipfrom').show();
                }

            }
        }
    });
    editPaperWin.show();
    initForm(row);
}
function initForm(row) {
    if (row != null) {
        var img = row.data.paperBanner.toString();
        var imgUrl = img.substring(img.lastIndexOf("\/") + 1);
        Ext.getCmp('paperBanner').setRawValue(imgUrl);
    }
}
function sformatDate(now) {
    var year = now.getFullYear();
    var month = now.getMonth()+1;
    var date = now.getDate();
    var hour = now.getHours();
    var minute = now.getMinutes();
    var second = now.getSeconds();
    return  new Date(year + "-" + month + "-" + date + "   " + "00:00:00");
};
function eformatDate(now) {
    //now = new Date(now.getFullYear() + "-" + +(now.getMonth() + 1) + "-" + +now.getDate() + "   " + +now.getHours() + ":" + now.getMinutes() + ":" + now.getSeconds());
    //var end = new Date(now.getTime() + 31 * 24 * 3600 * 1000);
    now.setMonth(now.getMonth()+1);
    //var year = now.getFullYear();
    //var month = now.getMonth();
    //var date = now.getDate();
    //return new Date(year + "-" + month + "-" + date);
    return now;
};

