<template>
  <span>{{ modelValue }}</span>
</template>

<script>
import dateUtil from "@/utils/date-util";

export default {
  name: "LocaleDateTime",
  components: {},
  props: ["value"],
  computed: {
    modelValue() {
      if (!this.value) {
        return "";
      }

      return dateUtil.getISODateTime(this.value);
    },
  },
};
</script>
