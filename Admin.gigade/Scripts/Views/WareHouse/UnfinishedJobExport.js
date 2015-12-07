Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);

Ext.onReady(function () {
    var exportExl = Ext.create('Ext.form.Panel', {
        frame: false,
        width:500,
        bodyPadding: 30,
        border: false,
        id: 'Import',
        items: [
         {
             html: '<div class="capion">提示：匯出缺貨明細～未完成理貨工作報表</div>',
             frame: false,
             border: false
         },
        {
            xtype: 'fieldcontainer',
            fieldLabel: '選擇',
            id: 'validate',
            width: 800,
            labelWidth: 90,
            defaultType: 'radiofield',
            submitValue: true,
            defaults: {
                flex: 1
            },
            layout: 'hbox',
            items: [
                    {
                        boxLabel: '製作總表',
                        name: 'check',
                        inputValue: '0',
                        width: 200,
                        id: 'radio1',
                        margins: '0 -530 0 0',
                        checked: true,
                        listeners: {
                            change: function (radio, newValue, oldValue) {
                                var assg_id = Ext.getCmp("assg_id");
                                if (newValue) {

                                    assg_id.setValue("");
                                    assg_id.setDisabled(true);

                                }
                            }
                        }
                    }
                   ,
                    {
                        xtype: 'fieldcontainer',
                        combineErrors: true,
                        layout: 'hbox',
                        margins: '0 10 0 0',
                        items: [
                                {
                                    boxLabel: '產生明細（缺貨報表）',
                                    xtype: 'radiofield',
                                    name: 'check',
                                    inputValue: '1',
                                    id: "radio2",
                                    listeners: {
                                        change: function (radio, newValue, oldValue) {
                                            var assg_id = Ext.getCmp("assg_id");
                                            if (newValue) {
                                                assg_id.setDisabled(false);

                                            }
                                        }
                                    }
                                },
                                {
                                    xtype: "textfield",
                                    id: 'assg_id',
                                    labelWidth: 150,
                                    disabled: true,
                                    width: 200,
                                    name: 'assg_id'

                                }
                        ]

                    },

            ]
        },
        {
            xtype: 'fieldcontainer',
            combineErrors: true,
            layout: 'hbox',
            items: [ { xtype: 'label', margin: '2 0 0 0', text: '生成理貨單時間:' },
                       {
                           xtype: "datefield",
                           editable: false,
                           margin: '0 0 0 5',
                           id: 'start_time',
                           name: 'start_time',
                           format: 'Y/m/d',
                           listeners: {
                               select: function (a, b, c) {
                                   var start = Ext.getCmp("start_time");
                                   var end = Ext.getCmp("end_time");
                                   if (end.getValue() == null) {
                                       end.setValue(setNextMonth(start.getValue(), 1));
                                   } else if (start.getValue() > end.getValue()) {
                                       Ext.Msg.alert(INFORMATION, "開始時間不能大於結束時間");
                                       end.setValue(setNextMonth(start.getValue(), 1));
                                   }
                                   //else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                                   //    // Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                   //    end.setValue(setNextMonth(start.getValue(), 1));
                                   //}
                               },
                               specialkey: function (field, e) {
                                   if (e.getKey() == e.ENTER) {
                                       Query();
                                   }
                               }
                           }
                       },
                       { xtype: 'displayfield', value: '~ ' },
                       {
                           xtype: "datefield",
                           editable: false,
                           id: 'end_time',
                           name: 'end_time',
                           format: 'Y/m/d',
                           listeners: {
                               select: function (a, b, c) {
                                   var start = Ext.getCmp("start_time");
                                   var end = Ext.getCmp("end_time");
                                   var s_date = new Date(start.getValue());
                                   var now_date = new Date(end.getValue());
                                   if (start.getValue() != "" && start.getValue() != null) {
                                       if (end.getValue() < start.getValue()) {
                                           Ext.Msg.alert(INFORMATION, "結束時間不能小於開始時間");
                                           end.setValue(setNextMonth(start.getValue(), 1));
                                       }
                                       //else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                                       //    //Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                       //    start.setValue(setNextMonth(end.getValue(), -1));
                                       //}

                                   } else {
                                       start.setValue(setNextMonth(end.getValue(), -1));
                                   }
                               },
                               specialkey: function (field, e) {
                                   if (e.getKey() == e.ENTER) {
                                       Query();
                                   }
                               }
                           }

                       }
            ]
        },
         {
             xtype: 'fieldcontainer',
             combineErrors: true,
             layout: 'hbox',
             items: [
     {
         xtype: 'button',
         text: '確定',
         width: 70,
         //style: {
         //    marginLeft: '50px'
         //},
         id: 'export', 
         handler: function () {
             window.open('/WareHouse/OutUndoneJobExl?radio1=' + Ext.getCmp('radio1').getValue() + '&radio2=' + Ext.getCmp('radio2').getValue() + '&assg_id=' + Ext.getCmp("assg_id").getValue() + "&starttime=" + Ext.Date.format(new Date(Ext.getCmp('start_time').getValue()), 'Y-m-d H:i:s') + "&endtime=" + Ext.Date.format(new Date(Ext.getCmp('end_time').getValue()), 'Y-m-d H:i:s'));
            
         }

     },
     {
         xtype: 'button',
         text: "重置",
         width: 70,
         //iconCls: 'icon-search',
         margin: '0 10 0 20',
         id: 'btnresult',
         iconCls: 'ui-icon ui-icon-reset',
         handler: function () {
             this.up('form').getForm().reset();
         }

     }
     ]}

        ]

    });
    setNextMonth = function (source, n) {
        var s = new Date(source);
        s.setMonth(s.getMonth() + n);
        if (n < 0) {
            s.setHours(0, 0, 0);
        } else if (n > 0) {
            s.setHours(23, 59, 59);
        }
        return s;
    };
    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [exportExl],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                // exportTab.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
});
