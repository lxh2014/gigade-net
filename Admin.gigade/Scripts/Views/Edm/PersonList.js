pageSize = 25;
var info_type = "edm_group_email";//資安數據來源的表
var secret_info = "user_id;user_name;user_email";//grid 列表頁顯示數據的列名
Ext.define('gigade.PersonList', {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'group_id', type: 'int' },
    { name: 'group_name', type: 'string' },
    { name: 'email_address', type: 'string' },
    { name: 'email_name', type: 'string' },
    { name: 'email_status', type: 'int' },
    ]
});
var PersonStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.PersonList',
    proxy: {
        type: 'ajax',
        url: '/Edm/GetPersonList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
PersonStore.on('beforeload', function () {
    Ext.apply(PersonStore.proxy.extraParams,
    {
        gid: Ext.getCmp('group_id').getValue(),
        gname: Ext.getCmp('group_name').getValue(),
        email_id: document.getElementById('email_id').value
    });
});
Ext.onReady(function () {
    var gdList = Ext.create('Ext.grid.Panel', {
        id: 'gdList',
        store: PersonStore,
        height: document.documentElement.clientHeight,
        columnLines: true,
        frame: true,
        columns: [
        { header: "群組編號", dataIndex: 'group_id', flex: 1, align: 'center' },
        { header: "群組名稱", dataIndex: 'group_name', flex: 1, align: 'center' },
        {
            header: "電子報訂閱狀態", dataIndex: 'email_status', flex: 1, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                if (value == 1) {
                    return "訂閱";
                } else if (value == 2) {
                    return "<span style=' color:red'>取消</span>";
                }
            }
        },
        {
            header: "電子郵件", dataIndex: 'email_address', flex: 2, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                return "<span onclick='SecretLogin(" + document.getElementById('email_id').value + "," + 0 + ",\"" + info_type + "\")'  >" + value + "</span>";
            }
        },
        {
            header: "姓名", dataIndex: 'email_name', flex: 1, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//secretcopy
                return "<span onclick='SecretLogin(" + document.getElementById('email_id').value + "," + 0 + ",\"" + info_type + "\")'  >" + value + "</span>";
            }
        }
        ],
        tbar: ['->',
        {
            xtype: 'textfield',
            id: 'group_id',
            name: 'group_id',
            value: '',
            labelWidth: 60,
            fieldLabel: '群組編號',
            value: '',
            editable: false,
            listeners: {
                specialkey: function (field, e) {
                    if (e.getKey() == e.ENTER) {
                        Query(1);
                    }
                }
            }
        },
        {
            xtype: 'textfield',
            id: 'group_name',
            name: 'group_name',
            value: '',
            labelWidth: 60,
            fieldLabel: '群組名稱',
            value: '',
            editable: false,
            listeners: {
                specialkey: function (field, e) {
                    if (e.getKey() == e.ENTER) {
                        Query(1);
                    }
                }
            }
        }, {
            xtype: 'button',
            margin: '0 5 0 5',
            iconCls: 'ui-icon ui-icon-search-2',
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
                    Ext.getCmp('group_id').setValue('');
                    Ext.getCmp('group_name').setValue('');
                }
            }
        }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: PersonStore,
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
        layout: 'fit',
        items: [gdList],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdList.width = document.documentElement.clientWidth;
                this.doLayout();
                PersonStore.load();
            }
        }
    });

})
function Query() {
    PersonStore.removeAll();
    var name = Ext.getCmp('group_name').getValue();
    var id = Ext.getCmp('group_id').getValue();
    if (name == "" && id == '') {
        Ext.Msg.alert(INFORMATION, '請選擇查詢條件');
    }
    else {
        Ext.getCmp("gdList").store.loadPage(1, {
            params: {
                gname: name,
                gid: id,
                email_id: document.getElementById('email_id').value
            }
        });
    }
}
function SecretLogin(rid, info_id, info_type) {//secretcopy
    var secret_type = "15";//參數表中的"訊息管理"
    var url = "/edm/PersonList";
    var ralated_id = rid;
    //點擊機敏信息先保存記錄在驗證密碼是否需要輸入
    boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
    if (boolPassword != "-1") {//不准查看
        if (boolPassword) {//超過5分鐘沒有輸入密碼
            //參數1：機敏頁面代碼，2：機敏資料主鍵，3：是否彈出驗證密碼框,4：是否直接顯示機敏信息6.驗證通過后是否打開編輯窗口
            //  function SecretLoginFun(type, relatedID, isLogin, isShow, editO, isEdit) {
            SecretLoginFun(secret_type, ralated_id, true, true, false, url, info_type, info_id, secret_info);//先彈出驗證框，關閉時在彈出顯示框

        } else {
            SecretLoginFun(secret_type, ralated_id, false, true, false, url, info_type, info_id, secret_info);//直接彈出顯示框
        }
    }
}

