

Ext.onReady(function () {
    Ext.create('Ext.form.Panel', {
        width: 600,
        border: false,
        bodyStyle: "padding:5px",
        layout: "column",
        items: [{
            columnWidth: .5,
            xtype: "panel",
            border: false,
            bodyStyle: " background-color:#0480C3;font-size:18px;color:white;",
            html: "點選以下按鈕，開始同步動作!"
        },
        {
            columnWidth: .5,
            xtype: "panel",
            border: false,
            items: [{
                xtype: "button",
                text: "開始同步",
                id: "btnadd",
                listeners: {
                    click: function () {
                        Ext.getCmp("btnadd").setText("請稍後..");
                        Ext.MessageBox.show({
                            msg: '數據同步中，請稍後....',
                            width: 300,
                            wait: true
                        });
                        Ext.Ajax.request({
                            url: "/Member/UserSyncToEdmList",
                            timeout: 900000,
                            success: function (response) {
                                var resultMessage = response.responseText;
                                Ext.Msg.alert('结果', resultMessage);
                              
                                //Ext.MessageBox.hide();
                                Ext.getCmp("btnadd").setText("開始同步");
                            }
                        })
                    }
                }
            }]
        }
],
        renderTo: Ext.getBody()
    });
    Ext.create('Ext.form.Panel', {
        bodyStyle: "padding:5px",
        html: "同步會員方式如下：<br/><br/>1.此同步功能，固定同步至電子報名單群組編號為【1】的群組名單中。<br/>2.若會員的資料有異動，系統會主動同步至電子報系統，如會員的訂閱狀態／真實姓名等。<br/>3.若會員狀態為簡易會員，系統將略過，不做同步動作。<br/>4.若會員信箱異動過，系統會新增一筆新的資料至電子報系統，但是電子報系統中的舊信箱記錄，不會刪除，所以可能會造成電子報的數量會比會員名單多。",
        renderTo: Ext.getBody()
    });
})
