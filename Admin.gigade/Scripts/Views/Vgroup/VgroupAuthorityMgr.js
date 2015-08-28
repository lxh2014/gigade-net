
function groupAuthority(groupId) {
    //權限store
    var AuthorityStore = Ext.create('Ext.data.Store', {
        fields: ['FunctionGroup', 'items'],
        proxy: {
            type: 'ajax',
            url: '/FunctionVGroup/GetFgroupAuthority',
            actionMethods: 'post',
            reader: {
                type: 'json'
            }
        }
    });
    //權限框
    var AuthView = Ext.create('Ext.view.View', {
        deferInitialRefresh: false,
        autoScroll: true,
        frame: false,
        plain: true,
        store: AuthorityStore,
        tpl: Ext.create('Ext.XTemplate',
            '<div id="AuthView">',
            '<tpl for="."><div class="group">',
                '<h2><div>{FunctionGroup}</div></h2>',
                '<dl>',
                '<tpl for="items">',
                    '<div class="name downline">',
                        '<input type="checkbox" id="{RowId}" onclick="pageCancel(this)" {[values.checked=="True"?"checked=checked":""]} />&nbsp;<label for="{RowId}">{Name}</label><br/>',
                        '<div class="tool {[values.tools.length>0?"downline":""]}" style="float:left;">',
                            '<tpl for="tools">',
                                '<input type="checkbox" id="{RowId}" onclick="toolClick(this)" {[values.checked=="True"?"checked=checked":""]} />&nbsp;<label for="{RowId}">{Name}</label> ',
                                '{[xindex % 5==0?"<br/>":""]}',
                            '</tpl>',
                        '</div>',
                    '</div>',
                '</tpl>',
                '<div style="clear:left"></div></dl>',
            '</div></tpl>',
            '</div>'
        ),
        onContainerClick: function (e) {
            var group = e.getTarget('h2', 3, true);

            if (group) {
                group.up('div').toggleCls('collapsed');
            }
        },
        itemSelector: 'dl',
        overItemCls: 'group-hover'
    });

    Ext.create('Ext.window.Window', {
        title: TOOL_AUTHORITY,
        items: [{
            xtype: 'panel',
            autoScroll: true,
            items: [AuthView]
        }],
        width: 500,
        height: document.documentElement.clientHeight * 400 / 783,
        layout: 'fit',
        closeAction: 'destroy',
        resizable: false,
        draggable: false,
        modal: true,
        bbar: [{
            xtype: 'checkbox',
            boxLabel: CHECK_ALL,
            listeners: {
                change: function (e, newValue) {
                    $('#AuthView').find('input:checkbox').each(function () {
                        if (newValue) {
                            $(this).attr('checked', 'checked');
                        }
                        else {
                            $(this).removeAttr('checked');
                        }
                    });
                }
            }
        },
        '->',
        {
            xtype: 'button',
            text: SUBMIT,
            handler: function () {
                var chks = $('#AuthView').find(':checked');
                if (chks.length == 0) {
                    Ext.Msg.alert(INFORMATION, NO_DATA);
                    return;
                }
                var mask;
                if (!mask) {
                    mask = new Ext.LoadMask(Ext.getBody());
                }
                mask.show();
                var rowIds = '';
                chks.each(function () {
                    rowIds += $(this).attr('id') + '|';
                });
                Ext.Ajax.request({
                    url: '/FunctionVGroup/SaveAuthority',
                    method: 'post',
                    params: { GroupId: groupId, rowID: rowIds },
                    success: function (form, action) {
                        mask.hide();
                        var result = Ext.decode(form.responseText);
                        Ext.Msg.alert(INFORMATION, SET_AUTHORTIY_SUCCESS);
                        if (result.success) {
                            AuthorityStore.load({ params: { RowId: groupId} });
                        }
                    },
                    failure: function () {
                        mask.hide();
                        Ext.Msg.alert(INFORMATION, SET_AUTHORTIY_FAILURE);
                    }
                })
            }
        }, {
            xtype: 'button',
            text: RESET,
            handler: function () {
                AuthorityStore.removeAll();
                AuthorityStore.load({ params: { RowId: groupId} });
            }
        }]
    }).show();

    AuthorityStore.load({ params: { RowId: groupId} });
};

//給予按鈕權限時，頁面權限同時給予
function toolClick(e) {
    var chk = $("#" + e.id);
    var prev = chk.parent().prevAll('input:checkbox');
    if (chk.attr('checked') && !prev.attr('checked')) {
        prev.attr('checked', 'checked');
    }
}
//頁面權限取消時，頁面按鈕權限同時取消
function pageCancel(e) {
    var chk = $("#" + e.id);
    chk.nextAll('.tool').children('input:checkbox').each(function () {
        $(this).removeAttr('checked');
    });
}