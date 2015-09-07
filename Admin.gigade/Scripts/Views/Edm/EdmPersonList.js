pageSize = 25;
var info_type = "edm_group_email";//資安數據來源的表
var secret_info = "user_id;user_name;user_email";//grid 列表頁顯示數據的列名
Ext.define('gigade.EdmPersonList', {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'email_id', type: 'int' },
    { name: 'email_address', type: 'string' },
    { name: 'email_name', type: 'string' },
    { name: 'group_count', type: 'int' },
    ]
});
var PersonListStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.EdmPersonList',
    proxy: {
        type: 'ajax',
        url: '/Edm/GetEdmPersonList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
PersonListStore.on('beforeload', function () {
    Ext.apply(PersonListStore.proxy.extraParams,
    {
        name: Ext.getCmp('name').getValue(),
        email: Ext.getCmp('email').getValue()
    });
});
Ext.onReady(function () {
    var gdList = Ext.create('Ext.grid.Panel', {
        id: 'gdList',
        store: PersonListStore,
        height: document.documentElement.clientHeight,
        columnLines: true,
        frame: true,
        columns: [
        { header: "郵箱編號", dataIndex: 'email_id', flex: 1, align: 'center' },
        {
            header: "會員郵箱", dataIndex: 'email_address', flex: 2, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {

                return "<span onclick='SecretLogin(" + record.data.email_id + "," + 0 + ",\"" + info_type + "\")'  >" + value + "</span>";
            }
        },
        {
            header: "會員姓名", dataIndex: 'email_name', flex: 1, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//secretcopy
                return "<span onclick='SecretLogin(" + record.data.email_id + "," + 0 + ",\"" + info_type + "\")'  >" + value + "</span>";
            }
        },
        {
            header: "所在群組數量", dataIndex: 'group_count', align: 'center', hidden: true, id: 'gcount', flex: 1,
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                return "<a href='javascript:void(0)' onclick='TranToPerson(" + record.data.email_id + ")'>" + value + "</a>"
            }
        },
        ],
        tbar: ['->',
               {
                   xtype: 'textfield',
                   id: 'search_id',
                   name: 'search_id',
                   value: '',
                   labelWidth: 60,
                   fieldLabel: '郵箱編號',
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
            id: 'name',
            name: 'name',
            value: '',
            labelWidth: 60,
            fieldLabel: '會員姓名',
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
            id: 'email',
            name: 'email',
            value: '',
            labelWidth: 60,
            fieldLabel: '會員郵箱',
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
                    Ext.getCmp('email').setValue('');
                    Ext.getCmp('name').setValue('');
                    Ext.getCmp('search_id').reset();
                }
            }
        }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: PersonListStore,
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
            }
        }
    });
    ToolAuthority();
})
function Query() {
    PersonListStore.removeAll();
    var name = Ext.getCmp('name').getValue();
    var email = Ext.getCmp('email').getValue();
    var search_id = Ext.getCmp("search_id").getValue();
    Ext.getCmp("gdList").store.loadPage(1, {
        params: {
            name: name,
            email: email,
            search_id: search_id
        }
    });
}
function TranToPerson(email_id) {
    var urlTran = '/Edm/PersonList?email_id=' + email_id;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#PersonList');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'PersonList',
        title: '人員詳情列表',
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}

function SecretLogin(rid, info_id, info_type) {//secretcopy
    var secret_type = "15";//參數表中的"訊息管理"
    var url = "/edm/EdmPersonList";
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