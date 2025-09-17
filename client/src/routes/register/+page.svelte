<script lang="ts">
	import RegisterForm from "$lib/components/register-form.svelte";
	let userType: 'patient-user' | 'professional-user' | null = $state(null);
</script>

<div class="grid min-h-svh lg:grid-cols-2">
	<div class="flex flex-col gap-4 p-6 md:p-10 h-full overflow-y-auto">
		<div class="flex flex-1 items-center justify-center">
			<div class="w-full max-w-xs">
				<h1>Você quer se cadastrar como:</h1>
				<div class="flex gap-4">
					<button
						class="border border-primary text-primary p-2 rounded-md"
						onclick={() => (userType = 'patient-user')}
					>
						Paciente
					</button>
					<button
						class="border border-primary text-primary p-2 rounded-md"
						onclick={() => (userType = 'professional-user')}
					>
						Profissional
					</button>
				</div>
				{#if userType !== null}
					<RegisterForm userSpecification={userType} />
				{/if}
			</div>
		</div>
	</div>
	<div class="bg-muted relative hidden lg:block">
		<img
			src="/images/login/login-background.avif"
			alt="placeholder"
			class="absolute inset-0 h-full w-full object-cover dark:brightness-[0.2] dark:grayscale"
		/>
	</div>
</div>
