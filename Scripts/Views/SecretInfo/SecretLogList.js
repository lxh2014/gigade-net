var pageSize = 25;
/**********************************************************************群組管理主頁面**************************************************************************************/

//群組管理Model
Ext.define('gigade.SecretLog', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "user_id", type: "int" },
        { name: "user_email", type: "string" },
        { name: "ipfrom", type: "string" },
        { name: "url", type: "string" },
        { name: "createdate", type: "string" },
        { name: "type", type: "int" },
         { name: "type_name", type: "string" },
            { name: "related_id", type: "int" },
            { name: "related_name", type: "string" },
        { name: "createdate", type: "string" },
        { name: "countIp", type: "string" }

    ]
});
//到Controller獲取數據
var SecretLogStore = Ext.create('Ext.data.Store', {
    autoDestroy: true, //自動消除
    pageSize: pageSize,
    model: 'gigade.SecretLog',
    proxy: {
        type: 'ajax',
        url: '/SecretInfo/GetSecretInfoLog',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'//總行數
        }
    }
});

SecretLogStore.on('beforeload', function () {
    var is_count = Ext.getCmp('is_count').getValue();
    if (is_count.is_count == '1') {
        Ext.getCmp('count').show();
        Ext.getCmp('sumtotal').show();
        Ext.getCmp('url').hide();
        Ext.getCmp('related_id').hide();
        //Ext.getCmp('related_name').hide();

        Ext.getCmp('createdate').hide();

        Ext.getCmp('countIp').show();
        var sismail = Ext.getCmp('ismail').getValue();
        switch (sismail.ismail) {
            case 0:
                Ext.getCmp('userId').show();
                Ext.getCmp('userName').show();
                Ext.getCmp('ipFrom').show();
                break;
            case 1:
                Ext.getCmp('userId').show();
                Ext.getCmp('userName').show();
                Ext.getCmp('ipFrom').hide();
                break;
            case 2:
                Ext.getCmp('userId').hide();
                Ext.getCmp('userName').hide();
                Ext.getCmp('ipFrom').show();
                break;
        }
    } else {
        Ext.getCmp('count').hide();
        Ext.getCmp('countIp').hide();
        Ext.getCmp('sumtotal').hide();
        Ext.getCmp('createdate').show();
        Ext.getCmp('url').show();
        Ext.getCmp('related_id').show();
        //Ext.getCmp('related_name').show();
    }


    Ext.apply(SecretLogStore.proxy.extraParams, {
        user_id: Ext.getCmp('user_id').getValue(),
        login_mail: Ext.getCmp('login_mail').getValue(),
        login_ipfrom: Ext.getCmp('login_ipfrom').getValue(),
        start_date: Ext.getCmp('start').getValue(),
        end: Ext.getCmp('end').getValue(),
        sumtotal: Ext.getCmp('sumtotal').getValue(),
        is_count: Ext.getCmp('is_count').getValue(),
        ismail: Ext.getCmp('ismail').getValue(),
        countClass: Ext.getCmp('countClass').getValue(),
        type: Ext.getCmp('type').getValue()
    });
});
//定義ddl的數據
var CountClassStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": '查看數據次數', "value": "1" },
        { "txt": '成功輸入密碼次數', "value": "2" }
    ]
});
/*********參數表model***********/
Ext.define("gigade.paraModel", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'parameterCode', type: 'string' },
        { name: 'parameterName', type: 'string' }
    ]
});


var secretTypeStore = Ext.create("Ext.data.Store", {
    model: 'gigade.paraModel',
    autoLoad: true,
    autoDestroy: true,
    proxy: {
        type: 'ajax',
        url: '/Parameter/QueryPara?paraType=secret_type',
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});
//查询
Query = function () {
    if (Ext.getCmp('user_id').getValue() != "" || Ext.getCmp('login_mail').getValue() != "" || Ext.getCmp('login_ipfrom').getValue() != "" || Ext.getCmp('sumtotal').getValue() != "" || Ext.getCmp('start').getValue() != null || Ext.getCmp('type').getValue() != "") {
        SecretLogStore.removeAll();
        SecretLogStore.loadPage(1, {
            params: {
                params: { start: 0, limit: pageSize }
            }
        });
    }
    else {
        Ext.Msg.alert(INFORMATION, SEARCH_LIMIT);
    }
}

Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 150,
        border: 0,
        bodyPadding: 10,
        width: document.documentElement.clientWidth,
        items: [
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'textfield',
                        id: 'user_id',
                        name: 'user_id',
                        margin: '0 5px',
                        fieldLabel: '用戶編號',
                        labelWidth: 80,
                        editable: false,
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == e.ENTER) {
                                    Query();
                                }
                            }
                        }
                    },
                    {
                        xtype: 'textfield',
                        id: 'login_mail',
                        name: 'login_mail',
                        margin: '0 5px',
                        labelWidth: 60,
                        fieldLabel: '用戶郵箱',
                        editable: false,
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == e.ENTER) {
                                    Query();
                                }
                            }
                        }
                    }, {
                        xtype: 'textfield',
                        id: 'login_ipfrom',
                        name: 'login_ipfrom',
                        margin: '0 5px',
                        fieldLabel: '來源IP',
                        labelWidth: 60,
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == e.ENTER) {
                                    Query();
                                }
                            }
                        }

                    },
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                     {
                         xtype: 'combobox',
                         editable: false,
                         fieldLabel: '類型',
                         labelWidth: 80,
                         margin: "0 5 0 5",
                         id: 'type',
                         store: secretTypeStore,
                         displayField: 'parameterName',
                         valueField: 'parameterCode',
                         value: 0,
                         emptyValue: '所有類型'
                     },
                    {
                        xtype: 'datetimefield',
                        id: 'start',
                        name: 'start',
                        fieldLabel: '日期區間',
                        labelWidth: 60,
                        margin: '0 5px 0 5',
                        width: 220,
                        format: 'Y-m-d H:i:s',
                        editable: false,
                        time: { hour: 00, min: 00, sec: 00 }//開始時間00：00：00
                         ,
                        listeners: {
                            select: function (a, b, c) {
                                var start = Ext.getCmp("start");
                                var end = Ext.getCmp("end");
                                if (end.getValue() == null) {
                                    end.setValue(setNextMonth(start.getValue(), 1));
                                }
                                else if (end.getValue() < start.getValue()) {
                                    Ext.Msg.alert(INFORMATION, DATA_TIP);
                                    start.setValue(setNextMonth(end.getValue(), -1));
                                }
                                else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                                    Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                    end.setValue(setNextMonth(start.getValue(), 1));
                                }
                            },
                            specialkey: function (field, e) {
                                if (e.getKey() == e.ENTER) {
                                    Query();
                                }
                            }
                        }

                    },
                    {
                        xtype: 'displayfield',
                        value: '~',
                        margin: '0 5px'
                    },
                    {
                        xtype: 'datetimefield',
                        id: 'end',
                        name: 'end',
                        margin: '0 5px',
                        width: 160,
                        format: 'Y-m-d H:i:s',
                        editable: false,
                        time: { hour: 23, min: 59, sec: 59 }//標記結束時間23:59:59
                        ,
                        listeners: {
                            select: function (a, b, c) {
                                var start = Ext.getCmp("start");
                                var end = Ext.getCmp("end");
                                if (start.getValue() != "" && start.getValue() != null) {
                                    if (end.getValue() < start.getValue()) {
                                        Ext.Msg.alert(INFORMATION, DATA_TIP);
                                        end.setValue(setNextMonth(start.getValue(), 1));
                                    }
                                    else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                                       // Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                        start.setValue(setNextMonth(end.getValue(), -1));
                                    }
                                }
                                else {
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

                   xtype: 'radiogroup',
                   fieldLabel: "是否統計",
                   labelWidth: 70,
                   id: 'is_count',
                   layout: 'hbox',
                   width: 330,
                   margin: '0 2 0 5',
                   colName: 'is_count',
                   name: 'is_count',
                   defaults: {
                       name: 'is_count'
                   },
                   columns: 2,
                   vertical: true,
                   items: [
                              {
                                  id: 'bt1',
                                  boxLabel: YES,
                                  inputValue: '1',
                                  width: 160,
                                  listeners: {
                                      change: function (radio, newValue, oldValue) {
                                          var count = Ext.getCmp("count");
                                          var countClass = Ext.getCmp("countClass");
                                          var sumtotal = Ext.getCmp("sumtotal");
                                          if (newValue) {
                                              count.show();
                                              countClass.show();
                                              sumtotal.show();

                                          }
                                      }
                                  }
                              },
                              {
                                  id: 'bt2',
                                  boxLabel: NO,
                                  checked: true,
                                  inputValue: '0',
                                  width: 160,
                                  listeners: {
                                      change: function (radio, newValue, oldValue) {
                                          var count = Ext.getCmp("count");
                                          var countClass = Ext.getCmp("countClass");
                                          var sumtotal = Ext.getCmp("sumtotal");
                                          if (newValue) {
                                              count.hide();
                                              countClass.hide();
                                              sumtotal.hide();
                                          }
                                      }
                                  }
                              }
                   ]
               },
                {
                    xtype: 'textfield',
                    id: 'sumtotal',
                    name: 'sumtotal',
                    margin: '0 5px',
                    fieldLabel: '統計次數≥',
                    labelWidth: 70,
                    hidden: true,
                    regex: /^\d+$/,
                    regexText: '請輸入數字',
                    listeners: {
                        specialkey: function (field, e) {
                            // e.HOME, e.END, e.PAGE_UP, e.PAGE_DOWN,
                            // e.TAB, e.ESC, arrow keys: e.LEFT, e.RIGHT, e.UP, e.DOWN
                            if (e.getKey() == e.ENTER) {
                                Query();
                            }
                        }
                    }
                },
                ]
            },
             {
                 xtype: 'fieldcontainer',
                 combineErrors: true,
                 hidden: true,
                 layout: 'hbox',
                 id: 'count',
                 width: 600,
                 items: [
                     {
                         xtype: 'displayfield',
                         margin: '0 5px 0 5px',
                         fieldLabel: '統計條件',
                         labelWidth: 60
                     },
                     {
                         xtype: 'radiogroup',
                         id: 'ismail',
                         name: 'ismail',
                         colName: 'ismail',
                         allowBlank: false,
                         columns: 3,
                         width: 250,
                         margin: '0 5 0 ',
                         items: [
                             {
                                 boxLabel: '郵箱&IP',
                                 name: 'ismail',
                                 id: 'alls',
                                 inputValue: 0,
                                 checked: true

                             }, {
                                 boxLabel: '登入郵箱',
                                 name: 'ismail',
                                 id: 'ygp',
                                 inputValue: 1

                             },
                         {
                             boxLabel: '來源IP',
                             name: 'ismail',
                             id: 'ngp',
                             inputValue: 2

                         }
                         ]
                     },
                      {
                          xtype: 'combobox',
                          editable: false,
                          fieldLabel: '統計分類',
                          labelWidth: 70,
                          margin: "0 5 0 5",
                          hidden: true,
                          id: 'countClass',
                          store: CountClassStore,
                          displayField: 'txt',
                          valueField: 'value',
                          value: 2
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
                            margin: '0 10 0 10',
                            iconCls: 'icon-search',
                            text: "查詢",
                            handler: Query
                        },
                        {
                            xtype: 'button',
                            text: '重置',
                            id: 'btn_reset',
                            iconCls: 'ui-icon ui-icon-reset',
                            listeners: {
                                click: function () {
                                    this.up('form').getForm().reset();
                                }
                            }
                        }
                    ]
                }
        ]
    });
    var gdFgroup = Ext.create('Ext.grid.Panel', {
        id: 'gdFgroup',
        store: SecretLogStore,
        width: document.documentElement.clientWidth,
        flex: 0.8,
        columnLines: true,
        frame: true,
        columns: [
            { xtype: 'rownumberer', header: '序號', width: 50, align: 'center' },
            { header: "用戶編號", id: 'userId', dataIndex: 'user_id', width: 120, align: 'center' },
            { header: "用戶郵箱", id: 'userName', dataIndex: 'user_email', width: 220, align: 'center' },
            { header: "來源IP", dataIndex: 'ipfrom', id: 'ipFrom', width: 180, align: 'center' },
            { header: "次數統計", dataIndex: 'countIp', id: 'countIp', width: 180, align: 'center', hidden: true },
              { header: "訪問時間", dataIndex: 'createdate', id: 'createdate', width: 180, align: 'center' },
            { header: "機敏資料類型", dataIndex: 'type_name', id: 'typename', width: 180, align: 'center' },
              { header: "機敏資料地址", dataIndex: 'url', id: 'url', flex: 1, minWidth: 240, align: 'center' },
            { header: "機敏數據編號", dataIndex: 'related_id', id: 'related_id', width: 120, align: 'center' }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: SecretLogStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        }
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [frm, gdFgroup],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdFgroup.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();

});
setNextMonth = function (source, n) {
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