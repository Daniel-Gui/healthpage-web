<script lang="ts">
	import RegisterFormUser from '$lib/components/register-form-user.svelte';
</script>

<div class="grid min-h-svh lg:grid-cols-2">
	<div class="flex h-full flex-col gap-4 overflow-y-auto p-6 md:p-10">
		<div class="flex flex-1 items-center justify-center">
			<div class="w-full max-w-xs">
                <RegisterFormUser/>
			</div>
		</div>
	</div>
	<div class="bg-muted relative hidden lg:block">
		<img
			src="/images/login/login-user-background.avif"
			alt="placeholder"
			class="absolute inset-0 h-full w-full object-cover dark:brightness-[0.2] dark:grayscale"
		/>
	</div>
</div>
