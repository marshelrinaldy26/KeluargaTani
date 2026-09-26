// Write your JavaScript code.
function baseInitComplete() {
  $(this.api().table().container()).css({
    display: "block",
    width: "100%",
  });
  this.api().columns.adjust();

  // Override global search to only trigger on Enter
  var api = this.api();
  var searchInput = $(api.table().container()).find('.dataTables_filter input');
  
  if (searchInput.length > 0) {
      searchInput.off(); // unbind default DataTables search event
      searchInput.on('keyup', function(e) {
          if (e.keyCode === 13 || e.key === "Enter") {
              api.search(this.value).draw();
          }
      });
  }
}

$.extend($.fn.dataTable.defaults, {
  language: {
      processing: "Memuat data...",
      lengthMenu: "Tampilkan _MENU_ data",
      zeroRecords: "Data tidak ditemukan",
      info: "Menampilkan _START_ sampai _END_ dari _TOTAL_ data",
      infoEmpty: "Menampilkan 0 sampai 0 dari 0 data",
      infoFiltered: "(disaring dari _MAX_ total data)",

      paginate: {
          first: '<i class="fa fa-angle-double-left"></i>',
          previous: '<i class="fa fa-angle-left"></i>',
          next: '<i class="fa fa-angle-right"></i>',
          last: '<i class="fa fa-angle-double-right"></i>'
      }
  },
  layout: {
    topStart: null,
    topEnd: null,
    bottomStart: {
      features: ["paging", "pageLength"],
      className: "dt-flex-container d-flex align-items-center gap-2"
    },
    bottomEnd: "info"
  },
  order: [],
  orderCellsTop: true,
  processing: true,
  serverSide: true,
  paging: true,
  pagingType: "full_numbers",
  pageLength: 10,
  lengthChange: true,
  scrollX: false,
  scrollY: false,
  scrollCollapse: false,
  initComplete: baseInitComplete
});

function ShowSearchRowWithDateAndBoolean({
  api,
  dateColumns = [],
  booleanColumns = [],
  dropdownColumns = {},
  colLimit = undefined,
  excluded = [],
  placeholders = [],
}) {
  const tableEl = api.table().node();
  

  if (!tableEl._booleanState) {
    tableEl._booleanState = {};
  }
  const booleanState = tableEl._booleanState;

  api.columns().every(function () {
    var column = this;
    const tableContainer = api.table().container();

    if (!column.visible()) {
      return;
    }

    let cell = $(tableContainer)
      .find("#filterRow th")
      .eq(column.index("visible"));

    cell.empty();

    if (column.index() > colLimit || excluded.includes(column.index())) {
      return;
    }

    // A. Boolean (Radio Buttons)
    if (booleanColumns.includes(column.index())) {
      let wrapper = document.createElement("div");
      wrapper.className = "d-flex justify-content-start gap-2";
      wrapper.style.width = "100%";
      wrapper.innerHTML = `
        <label class="mb-0" style="margin-right: 6px;"><input type="radio" name="bool_${column.index()}" value="true" > Ya</label>
        <label class="mb-0"><input type="radio" name="bool_${column.index()}" value="false"> Tidak</label>
        <button class="btn btn-danger btn-sm p-0 p-1 clear-bool ml-auto" id="clear_${column.index()}" title="Reset" style="visibility: hidden;"> <i class='material-icons '>close</i>
        </button>
      `;

      $(wrapper).on("change", "input[type=radio]", function () {
        booleanState[column.index()] = $(this).val();
        column.search($(this).val());
        api.draw();
      });

      $(cell).on("click", ".clear-bool", function (e) {
        e.preventDefault();
        e.stopPropagation();
        delete booleanState[column.index()];
        $(wrapper).find("input[type=radio]").prop("checked", false);
        $(this).css("visibility", "hidden");
        column.search("").draw();
      });

      $(cell).append(wrapper);

    // B. Date Picker
    } else if (dateColumns.includes(column.index())) {
      let input = document.createElement("input");
      input.type = "date";
      input.classList.add("form-control", "search-box-input");
      input.style.width = "100%";

      $(input).on("change", function () {
        column.search(this.value.trim());
        api.draw();
      });

      $(cell).append(input);

    // C. Dropdown
    } else if (dropdownColumns.hasOwnProperty(column.index())) {
      let select = document.createElement("select");
      select.classList.add("form-control", "search-box-input");
      select.style.width = "100%";

      let defaultOpt = document.createElement("option");
      defaultOpt.value = "";
      defaultOpt.text = "Semua";
      select.appendChild(defaultOpt);

      const options = dropdownColumns[column.index()];
      options.forEach((opt) => {
        let option = document.createElement("option");
        option.value = opt.value;
        option.text = opt.text;
        select.appendChild(option);
      });

      $(select).on("change", function () {
        column.search(this.value);
        api.draw();
      });

      $(cell).append(select);

    // D. Text Input
    } else {
      let input = document.createElement("input");
      input.type = "text";
      input.classList.add("search-box-input", "form-control");
      input.style.width = "100%";
        if (placeholders[column.index()]) { 
            input.placeholder = placeholders[column.index()];  //
        }

      $(input).on("keydown", function (e) {
        if (e.key === "Enter" || e.keyCode === 13) {
          e.preventDefault();
          column.search(this.value.trim());
          api.draw();
        }
      });

      $(cell).append(input);
    }

    $(cell)
      .click(function (e) {
        e.stopPropagation();
      })
      .on("keydown", "input, select", function (e) {
        if (e.key === "Enter" || e.keyCode === 13) {
          e.preventDefault();
        }
      });
  });

  api.on("draw.dt", function () {
    booleanColumns.forEach((colIdx) => {
      if (booleanState[colIdx]) {
        const tc = api.table().container();
        let radio_input = tc.querySelector(`thead input[name="bool_${colIdx}"][value="${booleanState[colIdx]}"]`);
        let clear_btn = tc.querySelector(`thead button[id="clear_${colIdx}"]`);
        if (radio_input) radio_input.checked = true;
        if (clear_btn) clear_btn.style.visibility = "visible";
      }
    });
  });
}


function loadingAnimation(url) {
    return $('<div id="loading" class="well"><img src="' + url + '" /> Loading...</div>').prependTo("body");
}
function removeLoadingAnimation() {
    $("#loading").remove();
}

function formatDate(value) {
    if (value == null || value == "") return "";
    let date = new Date(value);
    const day = date.toLocaleString("default", { day: "2-digit" });
    const month = date.toLocaleString("default", { month: "long" });
    const year = date.toLocaleString("default", { year: "numeric" });
    return day + " " + month + " " + year;
}

function JsonformatDate(value) {
    if (value == null) return "";
    let date = new Date(value);
    const day = date.toLocaleString("id-ID", { day: "2-digit" });
    const month = date.toLocaleString("id-ID", { month: "long" });
    const year = date.toLocaleString("id-ID", { year: "numeric" });
    return day + " " + month + " " + year;
}

function JsonformatDateTime(value) {
    if (value == null) return "";

    let date = new Date(value);

    const day = date.toLocaleString("id-ID", { day: "2-digit" });
    const month = date.toLocaleString("id-ID", { month: "long" });
    const year = date.toLocaleString("id-ID", { year: "numeric" });

    const hour = String(date.getHours()).padStart(2, "0");
    const minute = String(date.getMinutes()).padStart(2, "0");
    const second = String(date.getSeconds()).padStart(2, "0");

    return (
      day + " " + month + " " + year + ", " + hour + ":" + minute + ":" + second
    );
}

function convertToNumberFormat(nominal) {
    if (nominal == null || nominal == "") return "";
    let nominal_str = nominal.toString();
    let nominal_str_array = nominal_str.split("");
    let nominal_str_array_reverse = [];
    let j = 0;
    for (let i = nominal_str_array.length - 1; i >= 0; i--) {
      j += 1;
      nominal_str_array_reverse.push(nominal_str_array[i]);
      if (j == 3 && i != 0) {
        nominal_str_array_reverse.push(".");
        j = 0;
      }
    }
    let result = "";
    for (let i = nominal_str_array_reverse.length - 1; i >= 0; i--) {
      result += nominal_str_array_reverse[i];
    }
    return result;
}
function toDateInputValue(dateString) {
    if (!dateString) return "";
    return dateString.substring(0, 10);
}

$(document).on("keyup", "input[data-role=numerictextbox]", function () {
    var min = parseInt($(this).attr("min"));
    var max = parseInt($(this).attr("max"));
    var value = parseInt($(this).val());
    if (!isNaN(max)) {
        if (value > max) {
            Swal.fire('Error', "Nilai tidak boleh melebihi batas maksimal", 'error');
            $(this).val(max);
        }
    }
    if (!isNaN(min)) {
        if (value < min) {
            Swal.fire('Error', "Nilai tidak boleh kurang dari batas minimal", 'error');
            $(this).val(min);
        }
    }
});
$(function () {
    $.ajaxSetup({
        error: function (jqXHR, exception) {
            if (exception === 'abort') {
                return; // Jangan tampilkan error jika request dibatalkan/di-abort
            }
            if (jqXHR.status === 0) {
                Swal.fire('Error!', 'Not connect.\n Verify Network.', 'error');
            } else if (jqXHR.status == 404) {
                Swal.fire('Error!', 'Requested page not found. [404]', 'error');
            } else if (jqXHR.status == 500) {
                Swal.fire('Error!', 'Internal Server Error [500].', 'error');
            } else if (exception === 'parsererror') {
                Swal.fire('Error!', 'Requested JSON parse failed.', 'error');
            } else if (exception === 'timeout') {
                Swal.fire('Error!', 'Time out error.', 'error');
            } else if (exception === 'abort') {
                Swal.fire('Error!', 'Ajax request aborted.', 'error');
            } else {
                Swal.fire('Error!', 'Uncaught Error. Status = ' + jqXHR.status + '\n' + jqXHR.responseText, 'error');
            }
        }
    });
});
