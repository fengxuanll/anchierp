﻿var $vm = avalon.define({
    $id: "RepairOrderList",
    List: [],
    Search: {
        ReceptionById: "",
        Status: "",
        SettlementStatus: "",
        RepairOn: "",
        CompleteOn: "",
        SettlementOn: ""
    }
});

function selectSearchRepairOnFn() {
    $vm.Search.RepairOn = $("#txtSearchRepairOn").val();
}

function selectSearchCompleteOnFn() {
    $vm.Search.CompleteOn = $("#txtSearchCompleteOn").val();
}

function selectSearchSettlementOnFn() {
    $vm.Search.SettlementOn = $("#txtSearchSettlementOn").val();
}

$(function () {
    refreshListFn();
});

function refreshListFn(pageIndex) {
    var postData = $vm.Search.$model;
    postData.PageIndex = pageIndex || 0;
    $.ajax({
        url: "/Repair/List",
        type: "POST",
        data: postData,
        success: function (data) {
            $vm.List = data.Data;
            laypage({
                cont: 'divPager', //容器。值支持id名、原生dom对象，jquery对象,
                curr: data.PageIndex + 1,
                pages: data.TotalPage, //总页数
                groups: 3, //连续分数数0
                skip: true, //不显示上一页
                jump: function (obj, first) {
                    if (!first) {
                        refreshListFn(obj.curr - 1);
                    }
                }
            });
        }
    });
}

// 设置已完工
function setCompletedFn() {
    var idList = getSelectedIdListFn();
    if (idList.length == 0) {
        $.msg("请选择需要设置已完工的维修单。", "error");
        return false;
    }

    $.ajax({
        url: "/Repair/Complete",
        type: "POST",
        data: {
            idList: idList
        },
        success: function () {
            $.msg("设置已完工成功。", "success");
            refreshListFn();
        }
    });
}

// 获取全选数据
function getSelectedIdListFn() {
    var idArray = [];
    var $checkList = $(".table tbody tr td input[type=checkbox]:checked");
    $.each($checkList, function ($index, item) {
        var $checkItem = $(item);
        idArray.push($checkItem.val());
    });

    return idArray;
}

// 反结算维修单
function cancelOrderFn() {
    var idList = getSelectedIdListFn();
    if (idList.length == 0) {
        $.msg("请选择需要反结算的维修单。", "error");
        return false;
    }

    layer.confirm("是否确定需要反结算？<div class='c-red'>反结算会清除维修单，退回配件。</div>", function () {
        $.ajax({
            url: "/Repair/Cancel",
            type: "POST",
            data: {
                idList: idList
            },
            success: function () {
                $.msg("反结算完成。", "success");
                refreshListFn();
            }
        });
    });
}