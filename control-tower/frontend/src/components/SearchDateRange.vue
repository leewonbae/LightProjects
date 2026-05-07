<template>
  <div class="search-period form-group-wrapper">
    <div class="datetime-picker-wrapper form-group">
      <label>조회 기간</label>
      <div class="datetime-picker">
        <flat-pickr
          v-model="searchStartDateTime"
          class="form-control"
          :config="config"
          placeholder="시작 날짜를 선택해주세요."></flat-pickr>
      </div>

      <span class="tilde">~</span>

      <div class="datetime-picker">
        <flat-pickr
          v-model="searchEndDateTime"
          class="form-control"
          :config="config"
          placeholder="종료 날짜를 선택해주세요."></flat-pickr>
      </div>

      <!-- date time period button -->
      <div class="date-time-period-wrapper">
        <div class="date-time-period">
          <button id="today" type="button" name="period" value="-1" @click="getPeriodToday()">
            오늘
          </button>
        </div>
        <div class="date-time-period">
          <button id="week" type="button" name="period" value="-7" @click="getPeriod(-7)">
            1주일
          </button>
        </div>
        <div class="date-time-period">
          <button id="month" type="button" name="period" value="-30" @click="getPeriod(-30)">
            1개월
          </button>
        </div>
        <div class="date-time-period">
          <button id="months" type="button" name="period" value="-90" @click="getPeriod(-90)">
            3개월
          </button>
        </div>
      </div>
      <!-- // date time period button -->
    </div>
  </div>
</template>

<script>
// <!-- flatpickr -->
import flatPickr from "vue-flatpickr-component";
import "flatpickr/dist/flatpickr.css";
import ConfirmDatePlugin from "flatpickr/dist/plugins/confirmDate/confirmDate.js";

import dateUtil from "@/utils/date-util.js";

export default {
  name: "SearchDateRange",
  components: {
    flatPickr,
  },
  props: ["startDateTime", "endDateTime"],
  data() {
    return {
      config: {
        enableTime: true,
        wrap: true, // set wrap to true only when using 'input-group'
        altFormat: "Y-m-d H:i:S",
        altInput: true,
        dateFormat: "Y-m-d H:i:S",
        plugins: [new ConfirmDatePlugin()],
      },
    };
  },
  computed: {
    searchStartDateTime: {
      get() {
        return this.startDateTime;
      },
      set(value) {
        this.$emit("update:startDateTime", value);
      },
    },
    searchEndDateTime: {
      get() {
        return this.endDateTime;
      },
      set(value) {
        this.$emit("update:endDateTime", value);
      },
    },
  },
  methods: {
    getPeriod(val) {
      this.searchStartDateTime = dateUtil.getISODateTimeAddDay(new Date(), val);
    },
    getPeriodToday() {
      this.searchStartDateTime = dateUtil.getISODateTimeNow();
    },
  },
};
</script>
