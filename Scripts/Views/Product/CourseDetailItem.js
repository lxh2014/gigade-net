var COURSE_ID = 0, PRODUCT_ID = 0;
var courseDetailStore, productItemStore, courseDetailItemStore;
var courseDetailItemGrid;

Ext.define('gigade.coursedetailitem', {
    extend: 'Ext.data.Model',
    idProperty: 'Course_Detail_Item_Id',
    fields: [
         { name: 'Course_Detail_Item_Id', type: 'int' },
         { name: 'Course_Detail_Id', type: 'int' },
         { name: 'Item_Id', type: 'int' },
         { name: 'Ticket_Count', type: 'int' },
         { name: 'People_Count', type: 'int' }]
});

Ext.onReady(function () {
    COURSE_ID = window.parent.GetCourseId();
    PRODUCT_ID = window.parent.GetProductId();

    courseDetailStore = Ext.create('Ext.data.Store', {
        fields: [
             { name: 'Course_Detail_Id', type: 'int' },
             { name: 'Course_Detail_Name', type: 'string' },
             { name: 'Address', type: 'string' }
        ],
        proxy: {
            type: 'ajax',
            url: '/Course/GetCurriculDetail',
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });

    productItemStore = Ext.create('Ext.data.Store', {
        fields: [
             { name: 'Item_Id', type: 'int' },
             { name: 'Spec_Name_1', type: 'string' },
             { name: 'Spec_Name_2', type: 'string' },
              {
                  name: 'Spec_Name', type: 'string', convert: function (v, record) {
                      if (!record.data.Spec_Name_1 && !record.data.Spec_Name_2) {
                          return PRODUCT_NON_SPEC;
                      }
                      return record.data.Spec_Name_1 + ' ' + record.data.Spec_Name_2;
                  }
              }
        ],
        proxy: {
            type: 'ajax',
            url: '/Product/GetProItems',
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });

    courseDetailItemStore = Ext.create('Ext.data.Store', {
        model: 'gigade.coursedetailitem',
        proxy: {
            type: 'ajax',
            url: '/Product/CourseDetailItemQuery', //edit by wwei0216w 2015/3/13
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });

    courseDetailStore.load({ params: { course_id: COURSE_ID } });
    productItemStore.load({ params: { ProductId: PRODUCT_ID } });

    /******************** courseDetailItem ***********************/
    /**************** 課程設定 Grid ****************/
    courseDetailItemGrid = Ext.create('Ext.grid.Panel', {
        plugins: [{ ptype: 'cellediting' }],
        title: CURRICULUM_SETTING,
        store: courseDetailItemStore,
        margin: '0 30 0 0',
        x: 20,
        y: 20,
        width: 380,
        height: 300,
        columns: [
            { xtype: 'rownumberer', width: 30 },
            {
                text: '', menuDisabled: true, width: 40, align: 'center', xtype: 'actioncolumn',
                icon: '../../../Content/img/icons/cross.gif',
                handler: function (grid, rowIndex, colIndex) {
                    if (courseDetailItemStore.getAt(rowIndex).data.Ticket_Count) {
                        Ext.Msg.alert(POINT_OUT, THIS_CURRICULUM_IS_SELL_CANT_DELETE_MESSAGE);
                        return false;
                    }
                    else {
                        courseDetailItemStore.removeAt(rowIndex);
                    }
                }
            },
            {
                text: PRODUCT_DETAIL, dataIndex: 'Item_Id', flex: 1, menuDisabled: true,
                editor: {
                    xtype: 'combobox',
                    queryMode: 'local',
                    editable: false,
                    store: productItemStore,
                    displayField: 'Spec_Name',
                    valueField: 'Item_Id'
                },
                renderer: function (val) {
                    var record = productItemStore.findRecord('Item_Id', val);
                    if (record) {
                        return record.data.Spec_Name;
                    }
                    return val ? val : '';
                }
            },
            {
                text: CURRICULUM_DETAIL, dataIndex: 'Course_Detail_Id', flex: 1, menuDisabled: true,
                editor: {
                    xtype: 'combobox',
                    queryMode: 'local',
                    editable: false,
                    store: courseDetailStore,
                    displayField: 'Course_Detail_Name',
                    valueField: 'Course_Detail_Id'
                },
                renderer: function (val) {
                    var record = courseDetailStore.findRecord('Course_Detail_Id', val);
                    if (record) {
                        return record.data.Course_Detail_Name;
                    }
                    return val ? val : '';
                }
            }, {
                text: PEOPLE_COUNT, dataIndex: 'People_Count', menuDisabled: true, width: 50,
                editor: {
                    xtype: 'numberfield',
                    minValue: 1
                }
            }],
        tbar: [{
            text: NEW_ONE_ITEM, handler: function () {
                courseDetailItemGrid.getStore().add({});
            }
        }]
    });

    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        items: [courseDetailItemGrid],
        renderTo: Ext.getBody(),
        autoScroll: true
    });

    courseDetailItemStore.load({ params: { productId: PRODUCT_ID } });
});

//保存課存設定 edit by xiangwang0413w 2015/03/09
function save(functionid) {
    var asyncResult = false;
    var courseDetailItems = new Array();
    Ext.each(courseDetailItemStore.data.items, function () {
        courseDetailItems.push(this.data);
    });

    var removeRecords = new Array();
    Ext.each(courseDetailItemStore.removed, function () {
        removeRecords.push(this.data);
    });

    Ext.Ajax.request({
        url: '/Product/CourseDetailItemSave',
        method: 'POST',
        async: false,
        params: {
            itemStr: Ext.encode(courseDetailItems),
            delItemStr: Ext.encode(removeRecords),
            productId: PRODUCT_ID ? PRODUCT_ID : 0
        },
        success: function (response, opts) {
            var resText = Ext.decode(response.responseText);
            if (resText.success) {
                asyncResult = true;
                Ext.Msg.alert(POINT_OUT, SAVE_COMPLETE);
            }
            else {
                asyncResult = false;
                Ext.Msg.alert(POINT_OUT, SAVE_FAIL);
                window.parent.setMoveEnable(true);
            }
        },
        failure: function (response, opts) {
            Ext.Msg.alert(POINT_OUT, SAVE_FAIL);
            window.parent.setMoveEnable(true);
            asyncResult = false;
        }
    });
    return asyncResult;
}
