Ext.define("gigade.Site", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "Site_Id", type: "string" },
        { name: "Site_Name", type: "string" }]
});

var SiteStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Site',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/PromotionsDiscount/GetSite",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }

    }
});
//會員群組Model
Ext.define("gigade.VipGroup", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "group_id", type: "string" },
        { name: "group_name", type: "string" }]
});
//會員群組store
var VipGroupStore = Ext.create('Ext.data.Store', {
    model: 'gigade.VipGroup',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/PromotionsDiscount/GetVipGroup",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }

    }
});
editFunction = function (row, store) {

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


    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        url: '/PromotionAmountDiscounts/Save',
        labelWidth: 45,
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
               {
                   xtype: 'displayfield',
                   fieldLabel: "編號",
                   id: 'id',
                   hidden: true,
               },
            {
                xtype: 'textfield',
                fieldLabel: "活動名稱",
                id: 'name',
                name:'name',
                allowBlank: false,
            },
        {
            xtype: 'combobox',
            fieldLabel: "群組名稱",
            id: 'group_id',
            name: 'group_id',
            store: VipGroupStore,
            displayField: 'group_name',
            valueField:'group_id',
            allowBlank: false,
            lastQuery: '',
            value: "0"
        }, {
            xtype: 'numberfield',
            fieldLabel: "折扣",
            id: 'discount',
            name: 'discount',
            minValue: 0,
            maxValue:100,
            allowDecimals: false,
            allowBlank: false,
            listeners: {
                change: function (field, newValue, oldValue) {
                    if (newValue !== oldValue)
                    {
                        Ext.getCmp('discount').getEl().first().dom.innerHTML = "折扣" + ":<font style='color:red'>" + (newValue) + "%</font>";
                    }
                }
            }
        },
                            {
                                xtype: 'fieldcontainer',
                                defaults: {
                                    margin: '0 60 0 0'
                                },
                                items: [
                                       {
                                           xtype: "displayfield",
                                           value: "<span style='font-size:12px; color:#F00;'>折扣範例 : 8折請填20; 9折請填10 </span>"
                                       }
                                ]
                            },

                            {
                                xtype: 'numberfield',
                                fieldLabel: '滿額金額',
                                id: 'amount',
                                name: 'amount',
                                allowBlank: false,
                                minValue: 0,
                                maxValue: 99999999999,
                                allowDecimals:false,
                            },
                              {
                                  xtype: 'numberfield',
                                  fieldLabel: '滿額件數',
                                  id: 'quantity',
                                  name: 'quantity',
                                  allowBlank: false,
                                  minValue: 0,
                                  maxValue: 99999999999,
                                  allowDecimals: false,
                              },
                              {
                                  xtype: 'combobox',
                                  id: 'site',
                                  name: 'site',
                                  fieldLabel: 'web site 設定',
                                  multiSelect: true,
                                  store: SiteStore,
                                  displayField: 'Site_Name',
                                  valueField: 'Site_Id',
                                  queryMode: 'local',
                                  typeAhead: true,
                                  forceSelection: false,
                                  emptyText: "請選擇...",
                                  allowBlank: false,
                                  editable:false,
                              },
       {
            xtype: 'datetimefield',
            fieldLabel: "活動開始時間",
            allowBlank: false,
            editable: false,
            format: 'Y-m-d H:i:s',
            time: { hour: 00, min: 00, sec: 00 },//開始時間00：00：00
            id: 'start',
            name: 'start',
            vtype: 'daterange',//標記類型
            endDateField: 'end' //標記結束時間
        }, {
            xtype: 'datetimefield',
            fieldLabel: "活動結束時間",
            allowBlank: false,
            format: 'Y-m-d H:i:s',
            time: { hour: 23, min: 59, sec: 59 },//標記結束時間23:59:59
            editable: false,
            id: 'end',
            name: 'end',
            vtype: 'daterange',//標記類型
            startDateField: 'start' //標記開始時間
        }
        ],
        buttons: [{
            text: '保存',
            formBind: true,
            disabled: true,
            handler: function () {
              
                var form = this.up('form').getForm();
                if (Ext.getCmp('start').getValue() > Ext.getCmp('end').getValue())
                {
                    Ext.Msg.alert("提示信息","開始時間不能大於結束時間");
                    return false;
                }
                if (form.isValid()) {
                    this.disable();
                    form.submit({
                        params: {
                            id: Ext.htmlEncode(Ext.getCmp('id').getValue()),
                            name: Ext.htmlEncode(Ext.getCmp('name').getValue()),
                            group_id: Ext.htmlEncode(Ext.getCmp('group_id').getValue()),
                            discount: Ext.htmlEncode(Ext.getCmp('discount').getValue()),
                            amount: Ext.htmlEncode(Ext.getCmp('amount').getValue()),
                            quantity: Ext.htmlEncode(Ext.getCmp('quantity').getValue()),
                            site: Ext.htmlEncode(Ext.getCmp('site').getValue()),
                            start: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('start').getValue()), 'Y-m-d H:i:s')),
                            end: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('end').getValue()), 'Y-m-d H:i:s')),
                        },
                        success: function (form, action) {
                            Ext.Msg.alert("提示信息", "保存成功！");
                            editWin.close();
                            store.load();

                        },
                        failure: function (form, action) {
                                Ext.Msg.alert("提示信息", "保存失敗！");
                                editWin.close();
                        }
                    });
                }
            }

        }]
    });

    var editWin = Ext.create('Ext.window.Window', {
        id: 'editWin',
        title: '新增/編輯',
        iconCls: 'icon-user-add',
        width: 400,
        height: 320,
        layout: 'fit',
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        //labelWidth: 60,
        closable: false,
        items: [editFrm],
        tools: [
    {
        type: 'close',
        qtip: '是否關閉',
        handler: function (event, toolEl, panel) {
            Ext.MessageBox.confirm("確認信息", "是否關閉窗口", function (btn) {
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
                    editFrm.getForm().loadRecord(row);
                    //alert(row.data.site);
                    //Ext.getCmp('site').setValue(row.data.site);
                    SiteStore.load({
                        callback: function () {
                            var siteIDs = row.data.site.toString().split(',');
                            var arrTemp = new Array();
                            for (var i = 0; i < siteIDs.length; i++) {
                                arrTemp.push(SiteStore.getAt(SiteStore.find("Site_Id", siteIDs[i])));
                            }
                            Ext.getCmp('site').setValue(arrTemp);
                        }
                    })
                }
            }

        },

    });
    editWin.show();

};