﻿
function editFunction(RowID, Store) {
    var ID = null;
    if (RowID != null) {
        ID = RowID.data["sa_id"];
    }

    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,//True 为 Panel 填充画面,默认为false.
        plain: true,//不在选项卡栏上显示全部背景。
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 50,
        url: '/SiteManager/SaveSiteAnalytics',
        defaults: { anchor: "80%", msgTarget: "side", labelWidth: 100 },//side添加一个错误图标在域的右边，鼠标悬停上面时弹出显示消息。
        //这个布局将子元素的位置与父容器大小进行关联固定. 如果容器大小改变, 所有固定的子项将按各自的anchor 规则自动被重新渲染固定.
        items: [
        {
            xtype: 'datefield',
            fieldLabel: '日索引',
            id: 's_sa_date',
            name: 's_sa_date',
            margin: '10 0 10 0',
            format: 'Y-m-d',
            editable: false,
            disabled: RowID ? true : false,
            allowBlank: false,
            value: new Date(),
            listeners: {
                'select': function () {
                    var date = Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('s_sa_date').getValue()), 'Y-m-d'));
                    var today = (Ext.htmlEncode(Ext.Date.format(new Date(), 'Y-m-d')));
                    if (date > today) {
                        Ext.Msg.alert("提示信息", "當前選擇日期大於今日,請重新選擇");
                        Ext.getCmp('s_sa_date').setValue(new Date());
                    }
                }
            }
        },
        {
            xtype: 'numberfield',
            fieldLabel: '造訪數',
            name: 'sa_session',
            id: 'sa_session',
            margin: '10 0 10 0',
            maxValue: 2147483647,
            minValue: 0,
            allowBlank: false,
            hideTrigger: true,
            allowDecimals: false
        },
        {
            xtype: 'numberfield',
            fieldLabel: '使用者',
            name: 'sa_user',
            id: 'sa_user',
            margin: '10 0 10 0',
            maxValue: 2147483647,
            minValue: 0,
            allowBlank: false,
            hideTrigger: true,
            allowDecimals: false
        },
        {
            xtype: 'numberfield',
            fieldLabel: '瀏覽量',
            name: 'sa_pageviews',
            id: 'sa_pageviews',
            margin: '10 0 10 0',
            maxValue: 2147483647,
            minValue: 0,
            allowBlank: false,
            hideTrigger: true,
            allowDecimals: false
        },
        {
            xtype: 'numberfield',
            fieldLabel: '單次造訪頁數',
            name: 'sa_pages_session',
            id: 'sa_pages_session',
            margin: '10 0 10 0',
            maxValue: 2147483647,
            minValue: 0,
            allowBlank: false,
            hideTrigger: true,
            decimalPrecision: 2
        },
        {
            xtype: 'numberfield',
            fieldLabel: '跳出率',
            name: 'sa_bounce_rate',
            id: 'sa_bounce_rate',
            margin: '10 0 10 0',
            maxValue: 0.99999999999999,
            minValue: 0,
            allowBlank: false,
            hideTrigger: true
        },
        {
            xtype: 'numberfield',
            fieldLabel: '平均停留時間(s)',
            name: 'sa_avg_session_duration',
            id: 'sa_avg_session_duration',
            margin: '10 0 10 0',
            maxValue: 2147483647,
            minValue: 0,
            decimalPrecision: 5,
            allowBlank: false,
            hideTrigger: true
        }
        ],
        buttons: [
        {
            text: '保存',
            formBind: true,// 设置按钮与表单绑定，需在表单验证通过后才可使用
            handler: function () {
                var sa_date = Ext.getCmp("s_sa_date").getValue();
                var result1 = 0;
                if (RowID == null) {
                    $.ajax({
                        url: "/SiteManager/CheckSiteAnalytics",
                        data: {
                            "sa_date": Ext.htmlEncode(Ext.Date.format(sa_date, 'Y-m-d'))
                        },
                        type: "post",
                        type: 'text',
                        success: function (msg) {
                            if (msg.success == 'true') {
                                result1++;
                                Ext.Msg.alert(INFORMATION, '已存在該日索引');
                                Ext.getCmp("s_sa_date").setValue('');
                            }
                        },
                        error: function (msg) {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    });
                }
                if (result1 == 0) {
                    var form = this.up('form').getForm();//沿着 ownerCt 查找匹配简单选择器的祖先容器.
                    if (form.isValid()) {//这个函数会调用已经定义的校验规则来验证输入框中的值，如果通过则返回true
                        form.submit({
                            params: {
                                sa_id: ID,
                                sa_date: sa_date,
                                sa_session: Ext.htmlEncode(Ext.getCmp("sa_session").getValue()),
                                sa_user: Ext.htmlEncode(Ext.getCmp("sa_user").getValue()),

                                sa_pageviews: Ext.htmlEncode(Ext.getCmp("sa_pageviews").getValue()),
                                sa_pages_session: Ext.htmlEncode(Ext.getCmp("sa_pages_session").getValue()),
                                sa_bounce_rate: Ext.htmlEncode(Ext.getCmp("sa_bounce_rate").getValue()),
                                sa_avg_session_duration: Ext.htmlEncode(Ext.getCmp("sa_avg_session_duration").getValue()),


                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                if (result.success == "true") {
                                    Ext.Msg.alert(INFORMATION, "保存成功!");
                                    editWin.close();
                                    SiteAnalyticsListStore.removeAll();
                                    SiteAnalyticsListStore.load();
                                } else {
                                    Ext.Msg.alert(INFORMATION, FAILURE);
                                }
                            }
                        })
                    }
                }
            }
        }]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: RowID ? '編輯目標對' : '增加目標對',
        id: 'editWin',
        iconCls: RowID ? "icon-user-edit" : "icon-user-add",
        width: 400,
        height: 350,
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
             qtip: '關閉',
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
                    editFrm.getForm().loadRecord(RowID); //如果是編輯的話,加載選中行數據,载入一个 Ext.data.Model 到表单中
                }
                else {
                    editFrm.getForm().reset(); //如果不是編輯的話
                }
            }
        }
    });
    editWin.show();
}