Ext.require([
    'Ext.form.*'
]);
var searchStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
          { 'txt': '常溫', 'value': 'N' },
          { 'txt': '冷凍', 'value': 'F' },
    ]
});
Ext.onReady(function () {
    var formPanel = Ext.create('Ext.form.Panel', {
        frame: false,
        width: 980,
        bodyPadding: 30,
        border: false,
        items: [
            {
                html: '<div class="capion">提示：輸入工作代號走寄蒼流程，輸入訂單編號走調度流程</div>',
                frame: false,
                border: false
            },
            {
                xtype: 'textfield',
                name: 'number',
                id: 'number',
                labelWidth: 150,
                fieldLabel: '請輸入工作代號/訂單號',
                listeners: {
                    change: function () {
                        var number = Ext.getCmp("number").getValue();
                        var i = Number(number);
                        if (number == i)
                        {
                            if (number.length == 9) {
                                Ext.Ajax.request({
                                    url: "/WareHouse/Getfreight",
                                    method: 'post',
                                    type: 'text',
                                    params: {
                                        number: Ext.getCmp("number").getValue()
                                    },
                                    success: function (form, action) {
                                        var result = Ext.decode(form.responseText);
                                        if (result.success) {
                                            if (result.fre.length > 0) {
                                                Ext.getCmp("freight_set").setValue(result.fre);
                                            }
                                        }
                                    }
                                });
                            }
                        }
                    }
                }
            },
            {
                xtype: 'combobox',
                id: 'freight_set',
                name: 'freight_set',
                fieldLabel: '調度流程運送方式',
                displayField: 'txt',
                valueField: 'value',
                labelWidth: 70,
                editable: false,
                store: searchStore,
                value:"N"
            },
            {
                xtype: 'button',
                text: "確定",
                id: 'btnQuery',
                buttonAlign: 'center',
                handler: Query
            }
        ],
        renderTo: Ext.getBody()
    });

    function Query(x) {
        var number = Ext.getCmp("number").getValue();
        var i = Number(number);
        if (number == i)//如果成立.則說明number是int類型.訂單號 調度
        {
            if (number.length == 9) {
                Ext.Ajax.request({
                    url: "/WareHouse/GetAseldOrdCount",
                    method: 'post',
                    type: 'text',
                    params: {
                        number: Ext.getCmp("number").getValue()
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            if (result.data > 0) {
                                document.location.href = "/WareHouse/MarkTallyTW?number=" + number + "&freight_set=" + Ext.getCmp('freight_set').getValue();
                            }
                            else {
                                Ext.MessageBox.alert(INFORMATION,"訂單中沒有調度商品!");
                            }
                        }
                    }
                });
            }
            else {
                Ext.MessageBox.alert(INFORMATION,"請輸入正確的訂單號!");
            }
        }
        else //如果不成立.則說明number是string 類型.也就是工作代號 寄倉
        {
            if (number.length == 15) {
                Ext.Ajax.request({
                    url: "/WareHouse/GetAseldMasterAssgCount",
                    method: 'post',
                    type: 'text',
                    params: {
                        number: Ext.getCmp("number").getValue()
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            if (result.data > 0) {
                                document.location.href = "/WareHouse/MarkTallyWD?number=" + number;
                            }
                            else {
                                Ext.MessageBox.alert(INFORMATION,"工作編碼中沒有寄倉商品!");
                            }
                        }
                    }
                });
            }
            else {
                Ext.MessageBox.alert(INFORMATION,"請輸入正確的工作編號!");
            }
           
        }
    }
});
